namespace Blazorex;

public readonly struct WheelDelta
{
    public float DeltaX { get; init; }
    public float DeltaY { get; init; }
    public float ClientX { get; init; }
    public float ClientY { get; init; }
}