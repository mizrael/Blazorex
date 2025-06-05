namespace Blazorex;

public sealed record TextRendering : CanvasProperty<string>
{
    private TextRendering(string value)
        : base(value) { }

    public static readonly TextRendering Auto = new("auto");
    public static readonly TextRendering OptimizeSpeed = new("optimizeSpeed");
    public static readonly TextRendering OptimizeLegibility = new("optimizeLegibility");
    public static readonly TextRendering GeometricPrecision = new("geometricPrecision");
}
