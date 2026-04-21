using System.Collections;
using Graphical;
using Graphical.Animations;
using Graphical.Primitives;
using Graphical.Util;

namespace Datastructures;

public struct BHeapAnimationData
{
    public AnimatedGraphic Heap;
    public AnimatedGraphic FullArray;
}

public class VisualisedBHeap(
    int[] data,
    float width = 800,
    float height = 600,
    double animationTime = 0.5
) : IEnumerable<int>
{
    public int this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public int[] Data => data;

    public int Length = data.Length;
    public int End { get; set; } = data.Length;

    private AnimatedGraphic? _animatedChanges;
    private VisualisedArray? _fullArray;

    public void StartRecording()
    {
        _fullArray = new([.. data]); // Don't want to copy by reference to avoid side effects
        _fullArray.StartRecording();
        _animatedChanges = Render().Animate();
    }

    public void Swap(int a, int b)
    {
        if (a >= data.Length || b >= data.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(a),
                "One of the indices was out of range."
            );
        }
        (data[a], data[b]) = (data[b], data[a]);

        if (_animatedChanges is null)
        {
            return;
        }
        _fullArray!.Swap(a, b);

        Graphic render = Render();
        string aKey = $"bheap_{a}";
        string bKey = $"bheap_{b}";
        bool swapToEnd = false;
        Primitive? pa = render.Find(aKey);
        Primitive? pb = render.Find(bKey);

        if (pa is null || pb is null)
        {
            throw new InvalidOperationException("Something went very wrong");
        }

        if (b >= End)
        {
            swapToEnd = true;
        }

        Colour hlColour = new((float)0x11 / 0xff, (float)0xf9 / 0xff, (float)0x11 / 0xff); // #11f911
        Paint highlight = new(hlColour, hlColour);

        _animatedChanges = _animatedChanges
            .With(
                new ParallelAnimation([
                    new PaintAnimation($"{aKey}.bg", highlight, animationTime),
                    new PaintAnimation($"{bKey}.bg", highlight, animationTime),
                ])
            )
            .With(
                new ParallelAnimation([
                    new TransformAnimation(
                        aKey,
                        animationTime,
                        pa.Transform with
                        {
                            Translation = swapToEnd
                                ? new(width - 80 - ((data.Length - End) * 80), -80)
                                : pb.Transform.Translation,
                        },
                        Interpolation: InterpolationTypes.Cubic
                    ),
                    new TransformAnimation(
                        bKey,
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
                new ParallelAnimation([
                    new PaintAnimation(
                        $"{aKey}.bg",
                        new Paint(Colour.White, Colour.White),
                        animationTime
                    ),
                    new PaintAnimation(
                        $"{bKey}.bg",
                        new Paint(Colour.White, Colour.White),
                        animationTime
                    ),
                ])
            )
            .With(
                new ChangeKeys(new Dictionary<string, string> { { aKey, bKey }, { bKey, aKey } })
            );
    }

    public BHeapAnimationData GetRecording() =>
        new()
        {
            Heap =
                _animatedChanges
                ?? throw new InvalidOperationException("A recording must be started first"),
            FullArray = _fullArray!.GetRecording(),
        };

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
        int tHeight = Height(0);
        const float bgPadding = 80;
        float bgWidth = width + bgPadding;
        float bgHeight = height + bgPadding;
        return renderSubtree(0, 0, 0, 0, tHeight, height / tHeight)
            .WithRectangle(
                bgWidth,
                bgHeight,
                key: "bg",
                transform: Transform.Identity with
                {
                    Translation = new(-bgPadding / 2, bgHeight / 2),
                },
                paint: new Paint(Colour.Transparent, Colour.Transparent)
            );
    }

    private Graphic renderSubtree(
        int index,
        float x,
        float y,
        int depth,
        int tHeight,
        float yOffset
    )
    {
        if (index >= data.Length)
        {
            return new Graphic();
        }

        // TODO: fix lines
        // 1. z-index
        // 2. don't render when no child
        // 3. remove when child gets moved to end
        // 4. change keys after swap (maybe also animate something during swap like scale to 0 and then 1 again)
        return new Graphic()
            .With(
                new Line(
                    new(0, 0),
                    new(-(width / 4 / (depth + 1)), yOffset),
                    Key: $"line_{lChild(index)}",
                    Transform: Transform.Identity with
                    {
                        Translation = new(x - (width / 8 / (depth + 1)), y + (yOffset / 2)),
                    },
                    Paint: new Paint(Colour.White, Colour.White, 10)
                )
            )
            .With(
                new Line(
                    new(0, 0),
                    new(width / 4 / (depth + 1), yOffset),
                    Key: $"line_{rChild(index)}",
                    Transform: Transform.Identity with
                    {
                        Translation = new(x + (width / 8 / (depth + 1)), y + (yOffset / 2)),
                    },
                    Paint: new Paint(Colour.White, Colour.White, 10)
                )
            )
            .With(
                new Graphic(
                    [
                        new Circle(50, Key: "bg"),
                        new Text(
                            data[index].ToString(),
                            FontSize: 30,
                            Paint: new Paint(Colour.Black, Colour.Black)
                        ),
                    ],
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
                    y + yOffset,
                    depth + 1,
                    tHeight,
                    yOffset
                ).Primitives
            )
            .WithRange(
                renderSubtree(
                    rChild(index),
                    x + (width / 4 / (depth + 1)),
                    y + yOffset,
                    depth + 1,
                    tHeight,
                    yOffset
                ).Primitives
            );
    }

    private static int parent(int index) => (index - 1) / 2;

    private static int lChild(int index) => (2 * index) + 1;

    private static int rChild(int index) => (2 * index) + 2;

    public IEnumerator<int> GetEnumerator()
    {
        return ((IEnumerable<int>)data).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
