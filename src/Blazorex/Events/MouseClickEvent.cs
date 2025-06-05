using System.Text.Json.Serialization;
using Blazorex.Serialization;

namespace Blazorex;

[JsonConverter(typeof(MouseClickEventJsonConverter))]
public readonly struct MouseClickEvent(double clientX, double clientY, int button)
{
    public readonly int Button = button;

    public readonly double ClientX = clientX;

    public readonly double ClientY = clientY;

    public static readonly MouseClickEvent Zero = new(0, 0, 0);
}
