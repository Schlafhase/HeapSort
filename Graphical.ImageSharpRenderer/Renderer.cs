using System.Numerics;
using Graphical.Primitives;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = Graphical.Primitives.Rectangle;

namespace Graphical.ImageSharpRenderer;

// BUG: bounds calculation for the canvas is completely off (width doesn't seem to change at all and height is not enough for rotated rect at least)
public static class Renderer
{
    public static Image<Rgba32> Render(Graphic g)
    {
        List<(Primitive primitive, Image<Rgba32> img)> rendered =
        [
            .. g
                .Primitives.Select(p => (primitive: p, img: renderLocal(p)))
                .Where(x => x.img is not null)
                .Select(x => (x.primitive, img: x.img!)),
        ];

        if (rendered.Count == 0)
            return new Image<Rgba32>(1, 1);

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

        float cos = MathF.Abs(MathF.Cos(t.Rotation));
        float sin = MathF.Abs(MathF.Sin(t.Rotation));

        float extentX = (hw * cos) + (hh * sin);
        float extentY = (hw * sin) + (hh * cos);

        return (
            t.Translation.X - extentX,
            t.Translation.Y - extentY,
            t.Translation.X + extentX,
            t.Translation.Y + extentY
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
            if (SystemFonts.TryGet(family, out FontFamily ff))
                return ff.CreateFont(size);
        }
        FontFamily any = SystemFonts.Families.FirstOrDefault();
        return any.CreateFont(size);
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
