using System.Collections;
using Graphical;
using Graphical.Animations;
using Graphical.Primitives;
using Graphical.Util;

namespace Datastructures;

public class VisualisedArray(
    int[] data,
    float width = 800,
    float height = 600,
    Colour? barColor = null,
    float spacing = 2,
    double animationTime = 0.5
) : IEnumerable<int>
{
    public int this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public int Length = data.Length;

    private Colour BarColor => barColor ?? Colour.Blue;
    private Paint BarPaint => new(BarColor, Colour.Black);
    private AnimatedGraphic? _animatedChanges;

    public void StartRecording()
    {
        _animatedChanges = Render().Animate();
    }

    public void Swap(int a, int b)
    {
        (data[a], data[b]) = (data[b], data[a]);

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

        if (_animatedChanges is null)
        {
            return;
        }

        _animatedChanges = _animatedChanges
            .With(
                new ParallelAnimation([
                    new TransformAnimation(
                        $"array_{a}",
                        animationTime,
                        pa.Transform with
                        {
                            Translation = pb.Transform.Translation,
                        },
                        Interpolation: InterpolationTypes.Cubic
                    ),
                    new TransformAnimation(
                        $"array_{b}",
                        animationTime,
                        pb.Transform with
                        {
                            Translation = pa.Transform.Translation,
                        },
                        Interpolation: InterpolationTypes.Cubic
                    ),
                ])
            )
            .With(
                new ChangeKeys(
                    new Dictionary<string, string>
                    {
                        { $"array_{a}", $"array_{b}" },
                        { $"array_{b}", $"array_{a}" },
                    }
                )
            );
    }

    public AnimatedGraphic GetRecording() =>
        _animatedChanges
        ?? throw new InvalidOperationException("A recording must be started first");

    public Graphic Render()
    {
        Graphic g = new();
        if (data.Length == 0)
            return g;
        float barWidth = (width - ((data.Length + 1) * spacing)) / data.Length;
        double maxVal = data[^1];
        double minVal = data[^1];

        for (int i = 0; i < data.Length; i++)
        {
            double val = data[i];
            if (val > maxVal)
                maxVal = val;
            if (val < minVal)
                minVal = val;
        }

        double range = maxVal + 1 - minVal;
        if (range == 0)
            range = 1;

        for (int i = 0; i < data.Length; i++)
        {
            double val = data[i];
            float normalizedHeight = (float)((val + 1 - minVal) / range) * (height * 0.8f);
            float x = spacing + (i * (barWidth + spacing));

            Rectangle bar = new(
                barWidth,
                normalizedHeight,
                $"array_{i}",
                Transform.Identity with
                {
                    Translation = new(x, normalizedHeight / 2), // Coordinates describe the center
                },
                BarPaint
            );
            g = g.With(bar);
        }

        return g.WithRectangle(
            width,
            height,
            transform: Transform.Identity with
            {
                Translation = new(width / 2, height / 2),
            },
            paint: new Paint(Colour.Transparent, Colour.Transparent)
        );
    }

    public IEnumerator<int> GetEnumerator()
    {
        return ((IEnumerable<int>)data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
