using System.Linq;

namespace Blazorex;

public sealed record Filter : CanvasProperty<string>
{
    private Filter(string value)
        : base(value) { }

    public static Filter None() => new("none");

    public static Filter Url(string url) => new($"url({url})");

    public static Filter Blur(double px) => new($"blur({px:0.##}px)");

    public static Filter Brightness(double percent) => new($"brightness({percent:0.##}%)");

    public static Filter Contrast(double percent) => new($"contrast({percent:0.##}%)");

    public static Filter DropShadow(
        double offsetX,
        double offsetY,
        double blurRadius,
        string color
    ) => new($"drop-shadow({offsetX:0.##}px {offsetY:0.##}px {blurRadius:0.##}px {color})");

    public static Filter Grayscale(double percent) => new($"grayscale({percent:0.##}%)");

    public static Filter HueRotate(double deg) => new($"hue-rotate({deg:0.##}deg)");

    public static Filter Invert(double percent) => new($"invert({percent:0.##}%)");

    public static Filter Opacity(double percent) => new($"opacity({percent:0.##}%)");

    public static Filter Saturate(double percent) => new($"saturate({percent:0.##}%)");

    public static Filter Sepia(double percent) => new($"sepia({percent:0.##}%)");

    public static Filter Combine(params Filter[] filters) =>
        new(string.Join(" ", filters.Select(f => f.Value)));
}
