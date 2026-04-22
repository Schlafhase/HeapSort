using System.Numerics;

namespace Graphical.Primitives
{
    public abstract record Primitive
    {
        public string? Key;
        public Transform Transform;
        public Paint Paint;
        public int Z;

        public Primitive(
            string? key = null,
            Transform? transform = null,
            Paint? paint = null,
            int z = 0
        )
        {
            if (key?.Contains('.') is true)
            {
                throw new ArgumentException(
                    "Key cannot contain a '.' because they are used to address nested primitives inside composites",
                    nameof(key)
                );
            }
            Key = key;
            Transform = transform ?? Transform.Identity;
            Paint = paint ?? Defaults.Paint;
            Z = z;
        }
    }

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

    public record Paint(Colour Fill, Colour Stroke, float Width = 1f);

    public record Colour(float R, float G, float B, float A = 1f)
    {
        public static readonly Colour Black = new(0, 0, 0);
        public static readonly Colour White = new(1, 1, 1);
        public static readonly Colour Transparent = new(0, 0, 0, 0);
        public static readonly Colour Red = new(1, 0, 0);
        public static readonly Colour Green = new(0, 1, 0);
        public static readonly Colour Blue = new(0, 0, 1);

        /// <summary>Linearly interpolates between two colours.</summary>
        public static Colour Lerp(Colour a, Colour b, float t) =>
            new(
                a.R + ((b.R - a.R) * t),
                a.G + ((b.G - a.G) * t),
                a.B + ((b.B - a.B) * t),
                a.A + ((b.A - a.A) * t)
            );
    }
}
