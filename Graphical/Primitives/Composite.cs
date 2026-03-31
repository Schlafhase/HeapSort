namespace Graphical.Primitives
{
    public abstract record Composite(
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint)
    {
        public abstract IEnumerable<Primitive> GetPrimitives();
    }
}
