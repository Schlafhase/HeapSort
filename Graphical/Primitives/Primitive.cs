using System.Numerics;

namespace Graphical.Primitives
{
    public abstract record Primitive(string? Key, Transform Transform, Paint Paint);

    public record Transform(Vector2 Translation, float Rotation, Vector2 Scale)
    {
        public static readonly Transform Identity = new(Vector2.Zero, 0f, Vector2.One);

        public Transform Translated(Vector2 delta) =>
            this with
            {
                Translation = Translation + delta,
            };

        public Transform RotatedBy(float radians) => this with { Rotation = Rotation + radians };

        public Transform ScaledBy(Vector2 factor) => this with { Scale = Scale * factor };

        public Matrix3x2 ToMatrix()
        {
            Matrix3x2 t = Matrix3x2.CreateTranslation(Translation);
            Matrix3x2 r = Matrix3x2.CreateRotation(Rotation);
            Matrix3x2 s = Matrix3x2.CreateScale(Scale);

            return s * r * t;
        }
    }

    public record Paint(Color Fill, Color Stroke, float Width = 1f);

    public record Color(float R, float G, float B, float A = 1f)
    {
        public static readonly Color Black = new(0, 0, 0);
        public static readonly Color White = new(1, 1, 1);
        public static readonly Color Transparent = new(0, 0, 0, 0);
        public static readonly Color Red = new(1, 0, 0);
        public static readonly Color Green = new(0, 1, 0);
        public static readonly Color Blue = new(0, 0, 1);

        /// <summary>Linearly interpolates between two colours.</summary>
        public static Color Lerp(Color a, Color b, float t) =>
            new(
                a.R + ((b.R - a.R) * t),
                a.G + ((b.G - a.G) * t),
                a.B + ((b.B - a.B) * t),
                a.A + ((b.A - a.A) * t)
            );
    }
}
