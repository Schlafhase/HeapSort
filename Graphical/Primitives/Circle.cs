namespace Graphical.Primitives
{
    public record Circle(
        float Radius,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null,
        int Z = 0
    ) : Primitive(Key, Transform, Paint, Z);
}
