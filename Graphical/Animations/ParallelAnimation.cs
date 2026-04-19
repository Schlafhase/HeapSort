namespace Graphical.Animations;

public record ParallelAnimation(IEnumerable<Animation> Animations)
    : Animation(Animations.Max(a => a.Duration))
{
    public override Graphic Apply(Graphic g, double t)
    {
        // TODO: implement logic for animations with different durations
        Graphic ret = g;
        foreach (Animation a in Animations)
        {
            ret = a.Apply(ret, t);
        }
        return ret;
    }
}
