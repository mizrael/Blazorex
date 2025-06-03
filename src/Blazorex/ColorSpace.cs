namespace Blazorex;

public record ColorSpace : CanvasProperty<string>
{
    private ColorSpace(string value)
        : base(value) { }

    public static readonly ColorSpace Srgb = new("srgb");
    public static readonly ColorSpace DisplayP3 = new("display-p3");
}
