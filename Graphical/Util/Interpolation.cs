namespace Graphical.Util;

public delegate double InterpolationType(double a, double b, double t);

public static class InterpolationTypes
{
    public static double Lerp(double a, double b, double t)
    {
        return a + ((b - a) * t);
    }

    public static double Cubic(double a, double b, double t)
    {
        return Lerp(a, b, t < 0.5 ? 4 * t * t * t : 1 - (Math.Pow((-2 * t) + 2, 3) / 2));
    }
}
