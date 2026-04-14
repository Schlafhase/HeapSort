namespace Graphical.Primitives
{
    public record Text(
        string Content,
        float FontSize = 16f,
        string FontFamily = "JetBrainsMono Nerd Font",
        string? Key = null,
        Transform? Transform = null,
        Paint? Paint = null
    ) : Primitive(Key, Transform ?? Transform.Identity, Paint ?? Defaults.Paint);

    public static partial class GraphicExtensions
    {
        extension(Graphic g)
        {
            public Graphic WithText(
                string content,
                float fontSize = 16f,
                string fontFamily = "JetBrainsMono Nerd Font",
                string? key = null,
                Transform? transform = null,
                Paint? paint = null
            )
            {
                return g.With(new Text(content, fontSize, fontFamily, key, transform, paint));
            }
        }
    }
}
