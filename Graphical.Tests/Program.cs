using System.Diagnostics;
using Datastructures;
using Graphical;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Graphical.Primitives.Color;

Graphic g = new Graphic()
    .WithRectangle(1920, 1080)
    .WithText(
        "Hello Wordl",
        transform: Transform.Identity with
        {
            Scale = new(5),
        },
        paint: new Paint(Color.Black, Color.Black)
    );

Renderer.Render(g).SaveAsPng("out.png");

VisualisedArray<int> a = new([1, 2, 3, 4]);

Stopwatch sw = Stopwatch.StartNew();
Image<Rgba32> arrayImage = Renderer.Render(a.Render());
Console.WriteLine($"Rendering array took {sw.ElapsedMilliseconds} ms");
arrayImage.Save("array.png");
