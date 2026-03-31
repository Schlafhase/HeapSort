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
            using Image<Rgba32> primtiveImg = renderPrimitive(p);
            primtiveImg.Mutate(ctx =>
                ctx.Transform(new AffineTransformBuilder().AppendMatrix(p.Transform.ToMatrix()))
            );
            img.Mutate(ctx => ctx.DrawImage(primtiveImg, 1f));
        }

        return img;
    }

    private static Image<Rgba32> renderPrimitive(
        Primitive p,
        bool ignoreMissingImplementation = true
    )
    {
        switch (p)
        {
            case Rectangle r:
                return new Image<Rgba32>((int)r.Width, (int)r.Height, r.Paint.Fill.ToRgba32());
            case Triangle t:
                Image<Rgba32> canvas = new(
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
            default:
                return ignoreMissingImplementation
                    ? new Image<Rgba32>(1, 1)
                    : throw new NotImplementedException(
                        $"No rendering method implemented for {p.GetType().Name}"
                    );
        }
    }
}
