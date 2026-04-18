using Algorithms;
using Graphical;
using Graphical.Animations;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using Color = Graphical.Primitives.Color;

var heapsort = HeapSort.Sort([2, 5, 3, 1, 6, 7, 20, 8, 11]);
heapsort.Item2.RenderToFile("heapsorted.mp4", (g, p) => Renderer.Render(g).Save(p));

Graphic g = new();

g = g.WithRectangle(1000, 100, key: "rect1")
    .WithRectangle(10, 10, key: "rect", paint: new Paint(Color.Red, Color.Red))
    .WithText("Hello world", paint: new Paint(Color.Black, Color.Black));

var animated = g.Animate()
    .With(new TransformAnimation("rect", 2, Transform.Identity with { Translation = new(200, 0) }))
    .With(new TransformAnimation("rect1", 2, Transform.Identity with { Rotation = 2 }));

animated.RenderToFile("out2.mp4", (g, p) => Renderer.Render(g).Save(p));
