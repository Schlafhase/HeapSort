namespace Graphical.Primitives
{
    public record CompositePrimitive(
        IReadOnlyList<Primitive> Children,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);
}
