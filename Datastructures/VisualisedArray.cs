using Graphical;
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

    public Graphic Render()
    {
        if (data.Length == 0)
            return new Graphic();

        return _isNumeric ? renderNumeric() : renderGeneric();
    }

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
                $"bar_{i}",
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
