using System.Diagnostics;
using System.Numerics;
using Datastructures;
using Graphical;
using Graphical.Animations;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Graphical.Primitives.Color;

Graphic g = new Graphic()
    .WithRectangle(1920, 1080, key: "rect")
    .WithText(
        "Hello Wordl",
        transform: Transform.Identity with
        {
            Scale = new(5),
        },
        paint: new Paint(Color.Black, Color.Black)
    );

AnimatedGraphic animated = g.Animate()
    .With(
        new TransformAnimation(
            "rect",
            2,
            new Transform(Translation: new Vector2(0), Rotation: 2, Scale: new Vector2(0.2f))
        )
    )
    .With(new TransformAnimation("rect", 1, Transform.Identity with { Scale = new Vector2(0.5f) }));

VisualisedArray<int> a = new([1, 2, 3, 4]);

Stopwatch sw = Stopwatch.StartNew();
Image<Rgba32> arrayImage = Renderer.Render(a.Render());
Console.WriteLine($"Rendering array took {sw.ElapsedMilliseconds} ms");
arrayImage.Save("array.png");

const int fps = 30;

Graphic? currentFrame = animated.Advance(1d / fps);
int frameNo = 1;

while (currentFrame is not null)
{
    Console.WriteLine($"Rendering {frameNo}");
    Renderer.Render(currentFrame).Save($"frames/frame_{frameNo.ToString().PadLeft(5, '0')}.jpg");
    currentFrame = animated.Advance(1d / fps);
    frameNo++;
}
