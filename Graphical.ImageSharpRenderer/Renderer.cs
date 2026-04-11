using System.Numerics;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Rectangle = Graphical.Primitives.Rectangle;

namespace Graphical.ImageSharpRenderer;

public static class Renderer
{
    public static Image<Rgba32> Render(Graphic g, int width, int height)
    {
        Image<Rgba32> img = new(width, height);

        foreach (Primitive p in g.Primitives)
        {
            using Image<Rgba32>? primitiveImg = renderPrimitive(p);
            if (primitiveImg is null)
                continue;
            primitiveImg.Mutate(ctx =>
                ctx.Transform(new AffineTransformBuilder().AppendMatrix(p.Transform.ToMatrix()))
            );
            img.Mutate(ctx => ctx.DrawImage(primitiveImg, 1f));
        }

        return img;
    }

    private static Image<Rgba32> getCanvas(
        float left,
        float top,
        float right,
        float bottom,
        Transform t,
        out Vector2 offset
    )
    {
        int width = (int)(right - left);
        int height = (int)(bottom - top);
        offset = new(-left, -right);

        return new(width, height);
    }

    private static Image<Rgba32>? renderPrimitive(
        Primitive p,
        bool ignoreMissingImplementation = true
    )
    {
        Image<Rgba32> canvas;
        switch (p)
        {
            case Composite c:
                IEnumerable<(Primitive p, Image<Rgba32> img)> rendered = c.GetPrimitives()
                    .Select(p => (p, renderPrimitive(p)))
                    .OfType<(Primitive, Image<Rgba32>)>();

                canvas = new(rendered.Max(r => r.p.), rendered.Max(r => r.Bounds.Height));
                float left = rendered.Min(r => r.Bounds.)
                canvas.Mutate(ctx =>
                {
                    foreach (Image<Rgba32> render in rendered)
                    {
                        ctx.DrawImage(render, new Point(0, 0), 1f);
                    }
                });
                return canvas;

            case Rectangle r:
                if ((int)r.Width == 0 || (int)r.Height == 0)
                {
                    return null;
                }
                return new Image<Rgba32>((int)r.Width, (int)r.Height, r.Paint.Fill.ToRgba32());

            case Triangle t:
                canvas = new(
                    (int)((IEnumerable<Vector2>)[t.A, t.B, t.C]).Max(v => v.X),
                    (int)((IEnumerable<Vector2>)[t.A, t.B, t.C]).Max(v => v.Y)
                );
                canvas.Mutate(ctx =>
                    ctx.FillPolygon(
                        t.Paint.Fill.ToColor(),
                        t.A.ToPointF(),
                        t.B.ToPointF(),
                        t.C.ToPointF()
                    )
                );
                return canvas;

            case Text t:
                throw new NotImplementedException();
            default:
                return ignoreMissingImplementation
                    ? new Image<Rgba32>(1, 1)
                    : throw new NotImplementedException(
                        $"No rendering method implemented for {p.GetType().Name}"
                    );
        }
    }
}
