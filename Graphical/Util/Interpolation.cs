namespace Graphical.Util;

public delegate double InterpolationType(double a, double b, double t);

public static class InterpolationTypes
{
    public static double Lerp(double a, double b, double t)
    {
        return a + ((b - a) * t);
    }
}
