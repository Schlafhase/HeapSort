using System.Diagnostics;
using System.Numerics;
using Graphical.Primitives;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = Graphical.Primitives.Rectangle;

namespace Graphical.ImageSharpRenderer;

public static class Renderer
{
    public static Image<Rgba32> Render(Graphic g, float scale = 1)
    {
        List<(Primitive primitive, Image<Rgba32> img)> rendered =
        [
            .. g
                .Primitives.Select(p =>
                    (
                        primitive: p,
                        img: renderLocal(
                            p with
                            {
                                Transform = p.Transform with { Scale = p.Transform.Scale * scale },
                            }
                        )
                    )
                )
                .Where(x => x.img is not null)
                .Select(x => (x.primitive, img: x.img!)),
        ];

        if (rendered.Count == 0)
        {
            return new Image<Rgba32>(1, 1);
        }

        List<(float minX, float minY, float maxX, float maxY)> aabbs = rendered.ConvertAll(x =>
            worldAabb(x.primitive, x.img)
        );

        float minX = aabbs.Min(b => b.minX);
        float minY = aabbs.Min(b => b.minY);
        float maxX = aabbs.Max(b => b.maxX);
        float maxY = aabbs.Max(b => b.maxY);

        int canvasW = Math.Max(1, (int)MathF.Ceiling(maxX - minX));
        int canvasH = Math.Max(1, (int)MathF.Ceiling(maxY - minY));

        Image<Rgba32> canvas = new(canvasW, canvasH);

        foreach ((Primitive? primitive, Image<Rgba32>? img) in rendered)
        {
            img.Mutate(ctx =>
            {
                Transform t = primitive.Transform;
                if (t.Rotation != 0f || t.Scale != Vector2.One)
                {
                    // NOTE: Scaling is handled in renderLocal to avoid quality loss
                    ctx.Transform(new AffineTransformBuilder().AppendRotationRadians(t.Rotation));
                }
            });

            Vector2 dest =
                primitive.Transform.Translation
                - new Vector2(img.Width / 2f, img.Height / 2f)
                - new Vector2(minX, minY);

            canvas.Mutate(ctx => ctx.DrawImage(img, new Point((int)dest.X, (int)dest.Y), 1f));
        }

        return canvas;
    }

    private static (float minX, float minY, float maxX, float maxY) worldAabb(
        Primitive p,
        Image<Rgba32> localImg
    )
    {
        Transform t = p.Transform;

        float hw = localImg.Width / 2f * t.Scale.X;
        float hh = localImg.Height / 2f * t.Scale.Y;

        Vector2[] vertices = [new(-hw, hh), new(hw, hh), new(-hw, hh), new(-hw, -hh)];

        float cos = MathF.Abs(MathF.Cos(t.Rotation));
        float sin = MathF.Abs(MathF.Sin(t.Rotation));

        Vector2[] rotated =
        [
            .. vertices.Select(v => new Vector2(
                (v.X * cos) - (v.Y * sin),
                (v.X * sin) + (v.Y * cos)
            )),
        ];

        return (
            t.Translation.X + rotated.Min(v => v.X),
            t.Translation.Y + rotated.Min(v => v.Y),
            t.Translation.X + rotated.Max(v => v.X),
            t.Translation.Y + rotated.Max(v => v.Y)
        );
    }

    /// <summary>
    /// renderLocal renders one primitive on one canvas that
    /// should be just as big as the bounds of the primitives
    /// need to be. It also applies the scale of the primitives
    /// to avoid a loss of quality.
    /// </summary>
    private static Image<Rgba32>? renderLocal(Primitive p)
    {
        Vector2 scale = p.Transform.Scale;
        switch (p)
        {
            case Composite c:
                List<(Primitive child, Image<Rgba32> img)> children =
                [
                    .. c.GetPrimitives()
                        // PERF: could probably be optimised by merging both select calls but this is more readable
                        .Select(child =>
                            child with
                            {
                                Transform = child.Transform with
                                {
                                    Translation = child.Transform.Translation * scale,
                                    Scale = child.Transform.Scale * scale,
                                },
                            }
                        )
                        .Select(child => (child, img: renderLocal(child)!))
                        .Where(c => c.img is not null),
                ];

                if (children.Count == 0)
                    return null;

                List<(float minX, float minY, float maxX, float maxY)> childAabbs =
                    children.ConvertAll(x => worldAabb(x.child, x.img));

                float cMinX = childAabbs.Min(b => b.minX);
                float cMinY = childAabbs.Min(b => b.minY);
                float cMaxX = childAabbs.Max(b => b.maxX);
                float cMaxY = childAabbs.Max(b => b.maxY);

                int cW = Math.Max(1, (int)MathF.Ceiling(cMaxX - cMinX));
                int cH = Math.Max(1, (int)MathF.Ceiling(cMaxY - cMinY));

                Image<Rgba32> compositeImg = new(cW, cH);
                compositeImg.Mutate(ctx =>
                {
                    foreach ((Primitive? child, Image<Rgba32>? img) in children)
                    {
                        Vector2 dest =
                            child.Transform.Translation
                            - new Vector2(img.Width / 2f, img.Height / 2f)
                            - new Vector2(cMinX, cMinY);
                        ctx.DrawImage(img, new Point((int)dest.X, (int)dest.Y), 1f);
                    }
                });
                return compositeImg;

            case Rectangle r:
                int w = Math.Max(1, (int)(r.Width * scale.X));
                int h = Math.Max(1, (int)(r.Height * scale.Y));
                if (w == 0 || h == 0)
                    return null;

                return new Image<Rgba32>(w, h, r.Paint.Fill.ToRgba32());

            case Triangle t:
                Triangle scaled = t with { A = t.A * scale, B = t.B * scale, C = t.C * scale };

                float tMinX = new[] { scaled.A.X, scaled.B.X, scaled.C.X }.Min();
                float tMinY = new[] { scaled.A.Y, scaled.B.Y, scaled.C.Y }.Min();
                float tMaxX = new[] { scaled.A.X, scaled.B.X, scaled.C.X }.Max();
                float tMaxY = new[] { scaled.A.Y, scaled.B.Y, scaled.C.Y }.Max();

                int triW = Math.Max(1, (int)MathF.Ceiling(tMaxX - tMinX));
                int triH = Math.Max(1, (int)MathF.Ceiling(tMaxY - tMinY));

                if (triW == 0 || triH == 0)
                    return null;

                Vector2 offset = new(-tMinX, -tMinY);
                Image<Rgba32> tri = new(triW, triH);
                tri.Mutate(ctx =>
                    ctx.FillPolygon(
                        scaled.Paint.Fill.ToColor(),
                        (scaled.A + offset).ToPointF(),
                        (scaled.B + offset).ToPointF(),
                        (scaled.C + offset).ToPointF()
                    )
                );
                return tri;

            case Text t:
                return renderText(t);

            case Circle c:
                w = Math.Max(1, (int)(c.Radius * scale.X));
                h = Math.Max(1, (int)(c.Radius * scale.Y));
                if (w == 0 || h == 0)
                    return null;

                return new Image<Rgba32>(w, h, c.Paint.Fill.ToRgba32());

            default:
                return new Image<Rgba32>(1, 1);
        }
    }

    private static readonly string[] _fontFallbacks =
    [
        "Arial",
        "Liberation Sans",
        "DejaVu Sans",
        "FreeSans",
    ];

    private static Font resolveFont(string fontFamily, float size)
    {
        foreach (string family in (IEnumerable<string>)[fontFamily, .. _fontFallbacks])
        {
            if (tryGetFF(family, out FontFamily ff))
            {
                return ff.CreateFont(size);
            }
        }
        FontFamily any = SystemFonts.Families.First();
        return any.CreateFont(size);
    }

    private static readonly Dictionary<string, FontFamily> _fontCache = [];
    private static readonly FontCollection _fontCollection = new();
    private static bool _systemFontsAdded;

    private static bool tryGetFF(string family, out FontFamily ff)
    {
        if (!_systemFontsAdded)
        {
            _fontCollection.AddSystemFonts();
            _systemFontsAdded = true;
        }

        if (_fontCache.TryGetValue(family, out FontFamily _ff))
        {
            ff = _ff;
            return true;
        }

        if (_fontCollection.TryGet(family, out _ff))
        {
            ff = _ff;
            _fontCache[family] = ff;
            return true;
        }

        ProcessStartInfo psi = new()
        {
            FileName = "fc-list",
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        using (Process? p = Process.Start(psi))
        {
            if (p is null)
            {
                goto fail;
            }
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            foreach (string line in output.Split('\n'))
            {
                string[] parts = line.Split(':');

                try
                {
                    string[] names = [.. parts[1].Split(',').Select(n => n.Trim())];

                    if (names.Contains(family))
                    {
                        ff = _fontCollection.Add(parts[0]);
                        _fontCache[family] = ff;
                        return true;
                    }
                }
                catch { } // IndexOutOfRangeException means bad format in fc-list output; don't care
            }
        }

        fail:
        ff = default;
        return false;
    }

    private static Image<Rgba32>? renderText(Text t)
    {
        Vector2 scale = t.Transform.Scale;
        float scaledSize = t.FontSize * MathF.Max(scale.X, scale.Y);
        Font font = resolveFont(t.FontFamily, scaledSize);

        TextOptions opts = new(font);
        FontRectangle measured = TextMeasurer.MeasureAdvance(t.Content, opts);

        int w = Math.Max(1, (int)MathF.Ceiling(measured.Width));
        int h = Math.Max(1, (int)MathF.Ceiling(measured.Height));

        if (w == 0 || h == 0)
            return null;

        Image<Rgba32> img = new(w, h);
        img.Mutate(ctx => ctx.DrawText(t.Content, font, t.Paint.Fill.ToColor(), new PointF(0, 0)));
        return img;
    }
}
