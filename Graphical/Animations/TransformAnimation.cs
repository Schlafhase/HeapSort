using System.Numerics;
using Graphical.Primitives;
using Graphical.Util;

namespace Graphical.Animations;

public record TransformAnimation(
    string Target,
    double Duration,
    Transform NewTransform,
    InterpolationType? Interpolation = null
) : Animation(Duration)
{
    public override Graphic Apply(Graphic g, double t)
    {
        InterpolationType interpolate = Interpolation ?? InterpolationTypes.Lerp;
        return g.Modify(
            Target,
            p =>
                p with
                {
                    Transform = p.Transform with
                    {
                        Translation = new Vector2()
                        {
                            X = (float)interpolate(
                                p.Transform.Translation.X,
                                NewTransform.Translation.X,
                                t
                            ),
                            Y = (float)interpolate(
                                p.Transform.Translation.Y,
                                NewTransform.Translation.Y,
                                t
                            ),
                        },
                        Scale = new Vector2()
                        {
                            X = (float)interpolate(p.Transform.Scale.X, NewTransform.Scale.X, t),
                            Y = (float)interpolate(p.Transform.Scale.Y, NewTransform.Scale.Y, t),
                        },
                        Rotation = (float)interpolate(
                            p.Transform.Rotation,
                            NewTransform.Rotation,
                            t
                        ),
                    },
                }
        );
    }
}
