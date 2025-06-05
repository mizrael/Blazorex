namespace Blazorex;

public sealed record GlobalCompositeOperation : CanvasProperty<string>
{
    private GlobalCompositeOperation(string value)
        : base(value) { }

    public static readonly GlobalCompositeOperation SourceOver = new("source-over");
    public static readonly GlobalCompositeOperation SourceIn = new("source-in");
    public static readonly GlobalCompositeOperation SourceOut = new("source-out");
    public static readonly GlobalCompositeOperation SourceAtop = new("source-atop");
    public static readonly GlobalCompositeOperation DestinationOver = new("destination-over");
    public static readonly GlobalCompositeOperation DestinationIn = new("destination-in");
    public static readonly GlobalCompositeOperation DestinationOut = new("destination-out");
    public static readonly GlobalCompositeOperation DestinationAtop = new("destination-atop");
    public static readonly GlobalCompositeOperation Lighter = new("lighter");
    public static readonly GlobalCompositeOperation Copy = new("copy");
    public static readonly GlobalCompositeOperation Xor = new("xor");
    public static readonly GlobalCompositeOperation Multiply = new("multiply");
    public static readonly GlobalCompositeOperation Screen = new("screen");
    public static readonly GlobalCompositeOperation Overlay = new("overlay");
    public static readonly GlobalCompositeOperation Darken = new("darken");
    public static readonly GlobalCompositeOperation Lighten = new("lighten");
    public static readonly GlobalCompositeOperation ColorDodge = new("color-dodge");
    public static readonly GlobalCompositeOperation ColorBurn = new("color-burn");
    public static readonly GlobalCompositeOperation HardLight = new("hard-light");
    public static readonly GlobalCompositeOperation SoftLight = new("soft-light");
    public static readonly GlobalCompositeOperation Difference = new("difference");
    public static readonly GlobalCompositeOperation Exclusion = new("exclusion");
    public static readonly GlobalCompositeOperation Hue = new("hue");
    public static readonly GlobalCompositeOperation Saturation = new("saturation");
    public static readonly GlobalCompositeOperation Color = new("color");
    public static readonly GlobalCompositeOperation Luminosity = new("luminosity");
}
