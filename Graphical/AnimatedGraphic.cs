using System.Collections.Immutable;
using Graphical.Animations;

namespace Graphical;

public sealed class AnimatedGraphic(Graphic baseGraphic, IEnumerable<Animation>? animations = null)
{
    private readonly Graphic _baseGraphic = baseGraphic;
    private Graphic _currentBaseFrame = baseGraphic;
    private int _animationIndex;
    private double _currentAnimationStart;
    private double _time;

    public ImmutableList<Animation> Animations { get; } = animations?.ToImmutableList() ?? [];

    public AnimatedGraphic With(Animation animation) =>
        new(_baseGraphic, Animations.Add(animation));

    public AnimatedGraphic WithRange(IEnumerable<Animation> animations) =>
        new(_baseGraphic, Animations.AddRange(animations));

    public void Reset()
    {
        _time = 0;
        _currentBaseFrame = _baseGraphic;
        _animationIndex = 0;
    }

    /// <summary>
    /// Advances the animation for the given amount of seconds.
    /// Loops around after returning null once when the end of the animation is hit.
    /// </summary>
    /// <param name="dt">Delta-time in seconds</param>
    /// <returns>The frame before advancing or null if it's the final frame</returns>
    public Graphic? Advance(double dt)
    {
        if (_animationIndex >= Animations.Count)
        {
            Reset();
            return null;
        }
        Animation current = Animations[_animationIndex];
        double t = (_time - _currentAnimationStart) / current.Duration;
        t = t < 1 ? t : 1;

        Graphic currentFrame = current.Apply(_currentBaseFrame, t);

        if (t >= 1)
        {
            _currentBaseFrame = currentFrame;
            _currentAnimationStart += current.Duration;
            _animationIndex++;
            if (_animationIndex >= Animations.Count)
            {
                Reset();
                return null;
            }
            current = Animations[_animationIndex];
        }

        while (t >= 1)
        {
            t = (_time - _currentAnimationStart) / current.Duration;
            t = t < 1 ? t : 1;
            currentFrame = current.Apply(_currentBaseFrame, t);

            if (t >= 1)
            {
                _currentBaseFrame = currentFrame;
                _currentAnimationStart += current.Duration;
                _animationIndex++;
                if (_animationIndex >= Animations.Count)
                {
                    Reset();
                    return null;
                }
                current = Animations[_animationIndex];
            }
        }

        _time += dt;

        return currentFrame;
    }
}
