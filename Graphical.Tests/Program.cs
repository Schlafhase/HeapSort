using System.Numerics;
using Datastructures;
using Graphical;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using Color = Graphical.Primitives.Color;

Graphic g = new Graphic()
    .WithRectangle(1920, 1080)
    .WithRectangle(
        50,
        100,
        transform: new Transform(new(500, 20), (float)(Math.PI / 4), new(10, 7)),
        paint: new Paint(Color.Red, Color.Transparent)
    )
    .WithTriangle(
        new(0, 0),
        new(100, 100),
        new(300, 0),
        transform: new Transform(new(0, 0), (float)(Math.PI / 5), new Vector2(10)),
        paint: new Paint(Color.Blue, Color.Transparent)
    )
    .With(
        new CircleInTriangle(
            200,
            Transform: new Transform(new(400, 400), 0f, Vector2.One),
            Paint: new Paint(Color.Green, Color.Transparent)
        )
    );

Renderer.Render(g, 1920, 1080).SaveAsPng("out.png");

VisualisedArray<int> a = new([1, 2, 3, 4]);

Renderer.Render(a.Render(), 1920, 1080).SaveAsPng("array.png");
