using Blazorex.Serialization;
using System.Text.Json.Serialization;

namespace Blazorex;

[JsonConverter(typeof(MouseCoordsConverter))]
public readonly struct MouseCoords
{
    public MouseCoords(int clientX, int clientY) : this(clientX, clientY, 0, 0) { }

    public MouseCoords(int clientX, int clientY, int offsetX, int offsetY)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.OffsetX = offsetX;
        this.OffsetY = offsetY;
    }

    public readonly int ClientX;

    public readonly int ClientY;

    public readonly int OffsetX;

    public readonly int OffsetY;

    public static readonly MouseCoords Zero = new(0, 0, 0, 0);
}

[JsonConverter(typeof(MouseButtonDataConverter))]
public readonly struct MouseButtonData
{
    public MouseButtonData(int clientX, int clientY, int button)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.Button = button;
    }

    public readonly int Button;

    public readonly int ClientX;

    public readonly int ClientY;

    public static readonly MouseButtonData Zero = new(0, 0, 0);
}

[JsonConverter(typeof(WheelDeltaConverter))]
public readonly struct WheelDelta
{

    public WheelDelta(int clientX, int clientY, int deltaX, int deltaY)
    {
        this.ClientX = clientX;
        this.ClientY = clientY;
        this.DeltaX = deltaX;
        this.DeltaY = deltaY;
    }

    public readonly int DeltaX;

    public readonly int DeltaY;

    public readonly int ClientX;

    public readonly int ClientY;

    public static readonly WheelDelta Zero = new(0, 0, 0, 0);
}
