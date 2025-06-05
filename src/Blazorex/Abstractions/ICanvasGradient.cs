namespace Blazorex;

public interface ICanvasGradient
{
    ICanvasGradient AddColorStop(float offset, string color);
}
