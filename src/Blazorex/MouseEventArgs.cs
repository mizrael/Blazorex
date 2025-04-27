namespace Blazorex;

public readonly struct MouseCoords
{
    public int ClientX { get; init; }
    public int ClientY { get; init; }
    public int OffsetX { get; init; }
    public int OffsetY { get; init; }
}

public readonly struct MouseButtonData
{
    public int Button { get; init; }
    public int ClientX { get; init; }
    public int ClientY { get; init; }
}

public readonly struct WheelDelta
{
    public int DeltaX { get; init; }
    public int DeltaY { get; init; }
    public int ClientX { get; init; }
    public int ClientY { get; init; }
}