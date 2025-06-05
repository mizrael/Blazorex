namespace Blazorex;

public interface ICanvasPattern
{
    ICanvasPattern SetTransform(
        double a = 1,
        double b = 0,
        double c = 0,
        double d = 1,
        double e = 0,
        double f = 0
    );

    ICanvasPattern SetTransform(
        double m11,
        double m12,
        double m13,
        double m14,
        double m21,
        double m22,
        double m23,
        double m24,
        double m31,
        double m32,
        double m33,
        double m34,
        double m41,
        double m42,
        double m43,
        double m44
    );
}
