namespace Graphical.Primitives
{
    public record Rectangle(
        float Width,
        float Height,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);
}
