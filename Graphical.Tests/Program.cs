using System.Numerics;
using Graphical;
using Graphical.Animations;
using Graphical.ImageSharpRenderer;
using Graphical.Primitives;
using SixLabors.ImageSharp;
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
    .With(new TransformAnimation("rect", 1, Transform.Identity with { Scale = new Vector2(0.5f) }))
    .With(
        new TransformAnimation(
            "rect",
            1.5,
            Transform.Identity with
            {
                Translation = new Vector2(2000, 0),
            }
        )
    );

animated.RenderToFile("out.mp4", (g, path) => Renderer.Render(g).Save(path));
