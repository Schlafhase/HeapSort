namespace Graphical.Primitives
{
    public record TextPrimitive(
        string Content,
        float FontSize = 16f,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);
}
