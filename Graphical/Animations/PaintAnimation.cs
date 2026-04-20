using Graphical.Primitives;
using Graphical.Util;

namespace Graphical.Animations
{
    public record PaintAnimation(
        string Target,
        Paint NewPaint,
        double Duration,
        InterpolationType? Interpolation = null
    ) : Animation(Duration)
    {
        private Paint? _startPaint;
        private bool _notFound;

        public override Graphic Apply(Graphic g, double t)
        {
            InterpolationType interpolate = Interpolation ?? InterpolationTypes.Lerp;
            if (_notFound)
            {
                return g;
            }
            var tsdf = g.FindRecursive(Target);
            _startPaint ??= g.FindRecursive(Target).Value?.Paint;
            if (_startPaint is null)
            {
                _notFound = true;
                return g;
            }
            return g.Modify(
                Target,
                p =>
                    p with
                    {
                        Paint = new Paint(
                            interpolateColour(_startPaint.Fill, NewPaint.Fill, t, interpolate),
                            interpolateColour(_startPaint.Stroke, NewPaint.Stroke, t, interpolate),
                            (float)interpolate(_startPaint.Width, NewPaint.Width, t)
                        ),
                    }
            );
        }

        private static Colour interpolateColour(
            Colour a,
            Colour b,
            double t,
            InterpolationType interpolate
        ) =>
            new(
                (float)interpolate(a.R, b.R, t),
                (float)interpolate(a.G, b.G, t),
                (float)interpolate(a.B, b.B, t),
                (float)interpolate(a.A, b.A, t)
            );
    }
}
