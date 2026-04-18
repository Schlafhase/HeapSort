using System.Collections;
using Graphical;
using Graphical.Animations;
using Graphical.Primitives;
using Graphical.Util;

namespace Datastructures;

public class VisualisedBHeap<T>(
    T[] data,
    float width = 800,
    float height = 600,
    double animationTime = 0.5
) : IEnumerable<T>
{
    public T this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public int Length = data.Length;

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

    public int Height(int index)
    {
        if (index >= data.Length)
        {
            return -1;
        }
        return 1 + Math.Max(Height(lChild(index)), Height(rChild(index)));
    }

    public Graphic Render()
    {
        return renderSubtree(0, 0, 0, Height(0));
    }

    private Graphic renderSubtree(int index, float x, int depth, int tHeight)
    {
        if (index >= data.Length)
        {
            return new Graphic();
        }

        float y = depth / tHeight * height;

        return new Graphic()
            .With(
                new Circle(
                    5,
                    Key: $"bheap_{index}",
                    Transform: Transform.Identity with
                    {
                        Translation = new(x, y),
                    }
                )
            )
            .WithRange(
                renderSubtree(
                    lChild(index),
                    x - (width / 4 / (depth + 1)),
                    depth + 1,
                    tHeight
                ).Primitives
            )
            .WithRange(
                renderSubtree(
                    rChild(index),
                    x + (width / 4 / (depth + 1)),
                    depth + 1,
                    tHeight
                ).Primitives
            );
    }

    private static int parent(int index) => (index - 1) / 2;

    private static int lChild(int index) => (2 * index) + 1;

    private static int rChild(int index) => (2 * index) + 2;

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
