namespace Graphical.Animations;

/// <summary>
/// Should allow arbitrary modifications over time on Graphics
/// </summary>
/// <param name="Target">Key of the target primitive</param>
/// <param name="Duration">Duration in seconds</param>
public abstract record Animation(string Target, double Duration)
{
    /// <summary>
    /// Applies the animation to a Graphic at a given time t
    /// </summary>
    /// <param name="t">Time; ranges from 0 to 1</param>
    public abstract Graphic Apply(Graphic g, double t);
}
