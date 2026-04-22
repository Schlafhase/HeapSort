namespace Graphical.Primitives
{
    public record Rectangle(
        float Width,
        float Height,
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null,
        int Z = 0
    ) : Primitive(Key, Transform, Paint, Z);

    public static partial class GraphicExtensions
    {
        extension(Graphic g)
        {
            public Graphic WithRectangle(
                float width,
                float height,
                string? key = null,
                Transform? transform = null,
                Paint? paint = null
            )
            {
                return g.With(new Rectangle(width, height, key, transform, paint));
            }
        }
    }
}
