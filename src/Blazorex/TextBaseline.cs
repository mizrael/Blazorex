namespace Blazorex;

public record TextBaseline : CanvasProperty<string>
{
    private TextBaseline(string value) : base(value) { }

    public static readonly TextBaseline Alphabetic = new("alphabetic");
    public static readonly TextBaseline Top = new("top");
    public static readonly TextBaseline Hanging = new("hanging");
    public static readonly TextBaseline Middle = new("middle");
    public static readonly TextBaseline Ideographic = new("ideographic");
    public static readonly TextBaseline Bottom = new("bottom");
}
