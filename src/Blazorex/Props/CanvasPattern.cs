using Blazorex.Interop;
using Blazorex.Renderer;

namespace Blazorex;

internal sealed record CanvasPattern : CanvasProperty<int>, ICanvasPattern
{
    private readonly RenderContext2D _context;
    private readonly MarshalReference _marshalReference;

    internal CanvasPattern(RenderContext2D context, MarshalReference marshalReference)
        : base(marshalReference.Id)
    {
        _context = context;
        _marshalReference = marshalReference;
    }

    public ICanvasPattern SetTransform(
        double a = 1,
        double b = 0,
        double c = 0,
        double d = 1,
        double e = 0,
        double f = 0
    )
    {
        _context.Call(
            "setTransform",
            [
                _marshalReference with
                {
                    IsElementRef = false,
                    ClassInitializer = "DOMMatrix"
                },
                new double[] { a, b, c, d, e, f }
            ]
        );

        return this;
    }

    public ICanvasPattern SetTransform(
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
    )
    {
        _context.Call(
            "setTransform",
            [
                _marshalReference with
                {
                    IsElementRef = false,
                    ClassInitializer = "DOMMatrix"
                },
                new double[]
                {
                    m11,
                    m12,
                    m13,
                    m14,
                    m21,
                    m22,
                    m23,
                    m24,
                    m31,
                    m32,
                    m33,
                    m34,
                    m41,
                    m42,
                    m43,
                    m44
                }
            ]
        );

        return this;
    }
}
