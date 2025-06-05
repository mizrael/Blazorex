using System.Text.Json.Serialization;
using Blazorex.Serialization;

namespace Blazorex;

[JsonConverter(typeof(MouseMoveEventJsonConverter))]
public readonly struct MouseMoveEvent(
    double clientX,
    double clientY,
    double offsetX,
    double offsetY
)
{
    public MouseMoveEvent(double clientX, double clientY)
        : this(clientX, clientY, 0, 0) { }

    public readonly double ClientX = clientX;

    public readonly double ClientY = clientY;

    public readonly double OffsetX = offsetX;

    public readonly double OffsetY = offsetY;

    public static readonly MouseMoveEvent Zero = new(0, 0, 0, 0);
}
