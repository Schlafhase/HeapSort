using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Graphical.ImageSharpRenderer
{
    public static class ConversionExtensions
    {
        extension(Primitives.Color c)
        {
            public Rgba32 ToRgba32() => new(c.R, c.G, c.B, c.A);

            public Color ToColor() => new(c.ToRgba32());
        }

        extension(Vector2 v)
        {
            public PointF ToPointF() => new(v.X, v.Y);
        }
    }
}
