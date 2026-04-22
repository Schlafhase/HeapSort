using System.Numerics;

namespace Graphical.Primitives
{
    public record Triangle(
        Vector2 A,
        Vector2 B,
        Vector2 C,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null,
        int Z = 0
    ) : Primitive(Key, Transform, Paint, Z);

    public static partial class GraphicExtensions
    {
        extension(Graphic g)
        {
            public Graphic WithTriangle(
                Vector2 a,
                Vector2 b,
                Vector2 c,
                string? key = null,
                Transform? transform = null,
                Paint? paint = null
            )
            {
                return g.With(new Triangle(a, b, c, key, transform, paint));
            }
        }
    }
}
