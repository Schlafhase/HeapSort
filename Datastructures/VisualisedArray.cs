using Graphical;
using Graphical.Animations;
using Graphical.Primitives;

namespace Datastructures;

public class VisualisedArray<T>(
    T[] data,
    float width = 800,
    float height = 600,
    Color? barColor = null,
    float spacing = 2
)
{
    private static readonly bool _isNumeric =
        typeof(T) == typeof(int)
        || typeof(T) == typeof(float)
        || typeof(T) == typeof(double)
        || typeof(T) == typeof(long)
        || typeof(T) == typeof(short)
        || typeof(T) == typeof(byte)
        || typeof(T) == typeof(decimal);

    private Color BarColor => barColor ?? Color.Blue;
    private Paint BarPaint => new(BarColor, Color.Black);
    private AnimatedGraphic? _animatedChanges;

    public void StartRecording()
    {
        _animatedChanges = Render().Animate();
    }

    public Graphic Render()
    {
        if (data.Length == 0)
            return new Graphic();

        return _isNumeric ? renderNumeric() : renderGeneric();
    }

    public void Swap(int a, int b)
    {
        Graphic render = Render();
        Primitive? pa = render.Find($"array_{a}");
        Primitive? pb = render.Find($"array_{b}");

        if (pa is null || pb is null)
        {
            throw new ArgumentOutOfRangeException(
                nameof(a),
                "One of the indices was out of range."
            );
        }

        _animatedChanges = _animatedChanges
            .With(
                new ParallelAnimation([
                    new TransformAnimation(
                        $"array_{a}",
                        1,
                        pa.Transform with
                        {
                            Translation = pb.Transform.Translation,
                        }
                    ),
                    new TransformAnimation(
                        $"array_{b}",
                        1,
                        pb.Transform with
                        {
                            Translation = pa.Transform.Translation,
                        }
                    ),
                ])
            )
            .With(
                new ChangeKeys(new Dictionary<string, string> { { $"array_{a}", $"array_{b}" } })
            );
    }

    public AnimatedGraphic GetRecording() => _animatedChanges;

    private Graphic renderNumeric()
    {
        Graphic g = new();
        float barWidth = (width - ((data.Length + 1) * spacing)) / data.Length;
        double maxVal = getNumericValue(data[^1]);
        double minVal = getNumericValue(data[^1]);

        for (int i = 0; i < data.Length; i++)
        {
            double val = getNumericValue(data[i]);
            if (val > maxVal)
                maxVal = val;
            if (val < minVal)
                minVal = val;
        }

        double range = maxVal - minVal;
        if (range == 0)
            range = 1;

        for (int i = 0; i < data.Length; i++)
        {
            double val = getNumericValue(data[i]);
            float normalizedHeight = (float)((val - minVal) / range) * (height * 0.8f);
            float x = spacing + (i * (barWidth + spacing));

            Rectangle bar = new(
                barWidth,
                normalizedHeight,
                $"array_{i}",
                Transform.Identity with
                {
                    Translation = new(x, 0),
                },
                BarPaint
            );
            g = g.With(bar);
        }

        return g;
    }

    private Graphic renderGeneric()
    {
        Graphic g = new();
        float boxSize = Math.Min(
            (width - ((data.Length + 1) * spacing)) / data.Length,
            height * 0.5f
        );

        for (int i = 0; i < data.Length; i++)
        {
            float x = spacing + (i * (boxSize + spacing));
            float y = (height - boxSize) / 2;

            g = g.With(
                new Rectangle(
                    boxSize,
                    boxSize,
                    $"box_{i}",
                    Transform.Identity with
                    {
                        Translation = new(x, y),
                    }
                )
            );
        }

        return g;
    }

    private static double getNumericValue(T value)
    {
        if (value is IConvertible convertible)
            return convertible.ToDouble(null);
        return 0;
    }
}
