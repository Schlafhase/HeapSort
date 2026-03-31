namespace Graphical.Primitives
{
    public record Circle(
        float Radius,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);
}
