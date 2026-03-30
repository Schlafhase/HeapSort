using System.Numerics;

namespace Graphical.Primitives
{
    public record TrianglePrimitive(
        Vector2 A,
        Vector2 B,
        Vector2 C,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);
}
