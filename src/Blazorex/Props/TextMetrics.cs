namespace Blazorex;

public sealed record TextMetrics
{
    public float Width { get; init; }
    public float ActualBoundingBoxLeft { get; init; }
    public float ActualBoundingBoxRight { get; init; }
    public float FontBoundingBoxAscent { get; init; }
    public float FontBoundingBoxDescent { get; init; }
    public float ActualBoundingBoxAscent { get; init; }
    public float ActualBoundingBoxDescent { get; init; }
    public float EmHeightAscent { get; init; }
    public float EmHeightDescent { get; init; }
    public float HangingBaseline { get; init; }
    public float AlphabeticBaseline { get; init; }
    public float IdeographicBaseline { get; init; }
}
