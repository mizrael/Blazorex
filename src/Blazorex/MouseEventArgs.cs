using Blazorex.Serialization;
using System.Text.Json.Serialization;

namespace Blazorex;

[JsonConverter(typeof(MouseCoordsConverter))]
public readonly struct MouseCoords
{
    public MouseCoords(double clientX, double clientY) : this(clientX, clientY, 0, 0) { }

    public MouseCoords(double clientX, double clientY, double offsetX, double offsetY)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.OffsetX = offsetX;
        this.OffsetY = offsetY;
    }

    public readonly double ClientX;

    public readonly double ClientY;

    public readonly double OffsetX;

    public readonly double OffsetY;

    public static readonly MouseCoords Zero = new(0, 0, 0, 0);
}

[JsonConverter(typeof(MouseButtonDataConverter))]
public readonly struct MouseButtonData
{
    public MouseButtonData(double clientX, double clientY, int button)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.Button = button;
    }

    public readonly int Button;

    public readonly double ClientX;

    public readonly double ClientY;

    public static readonly MouseButtonData Zero = new(0, 0, 0);
}

[JsonConverter(typeof(WheelDeltaConverter))]
public readonly struct WheelDelta
{

    public WheelDelta(double clientX, double clientY, double deltaX, double deltaY)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }

    public readonly double DeltaX;

    public readonly double DeltaY;

    public readonly double ClientX;

    public readonly double ClientY;

    public static readonly WheelDelta Zero = new(0, 0, 0, 0);
}
