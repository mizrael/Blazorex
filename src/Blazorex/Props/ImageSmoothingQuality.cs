namespace Blazorex;

public sealed record ImageSmoothingQuality : CanvasProperty<string>
{
    private ImageSmoothingQuality(string value)
        : base(value) { }

    public static readonly ImageSmoothingQuality Low = new("low");
    public static readonly ImageSmoothingQuality Medium = new("medium");
    public static readonly ImageSmoothingQuality High = new("high");
}
