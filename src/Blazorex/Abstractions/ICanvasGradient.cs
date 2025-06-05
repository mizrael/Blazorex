namespace Blazorex;

/// <summary>
/// Represents a gradient object used for canvas fill and stroke styles, as defined by the HTML5 Canvas API.
/// Gradients are created using methods such as <c>CreateLinearGradient</c>, <c>CreateRadialGradient</c>, or <c>CreateConicGradient</c>
/// and can have multiple color stops added to define the gradient transitions.
/// </summary>
public interface ICanvasGradient
{
    /// <summary>
    /// Adds a color stop to the gradient at the specified offset.
    /// </summary>
    /// <param name="offset">
    /// A number between 0.0 and 1.0 representing the position between the start and end of the gradient.
    /// 0.0 represents the start, 1.0 represents the end.
    /// </param>
    /// <param name="color">
    /// A CSS color value to be used at the specified offset.
    /// </param>
    /// <returns>
    /// The current <see cref="ICanvasGradient"/> instance for chaining.
    /// </returns>
    /// <remarks>
    /// This method corresponds to the <c>addColorStop()</c> method of the HTML5 CanvasGradient interface.
    /// </remarks>
    ICanvasGradient AddColorStop(float offset, string color);
}
