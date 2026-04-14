using Datastructures;
using Graphical;
using Graphical.Animations;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
using Color = Graphical.Primitives.Color;

VisualisedArray<int> varray = new([1, 2, 3, 4, 5]);

varray.StartRecording();
varray.Swap(1, 3);
varray.Swap(1, 4);

varray.GetRecording().RenderToFile("out.mp4", (g, p) => Renderer.Render(g).Save(p));

Graphic g = new();

g = g.WithRectangle(1000, 100)
    .WithRectangle(10, 10, key: "rect", paint: new Paint(Color.Red, Color.Red))
    .WithText("Hello world", paint: new Paint(Color.Black, Color.Black));

var animated = g.Animate()
    .With(new TransformAnimation("rect", 2, Transform.Identity with { Translation = new(200, 0) }));

animated.RenderToFile("out2.mp4", (g, p) => Renderer.Render(g).Save(p));
