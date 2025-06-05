namespace Blazorex;

/// <summary>
/// Represents a pattern object used for canvas fill and stroke styles, as defined by the HTML5 Canvas API.
/// Patterns are created using <c>IRenderContext.CreatePattern</c> and can have a transformation matrix applied.
/// </summary>
public interface ICanvasPattern
{
    /// <summary>
    /// Sets the transformation matrix that will be applied to the pattern when rendering.
    /// This overload uses a 2D affine transformation matrix, matching the <c>DOMMatrixInit</c> or 2D matrix form in the HTML5 Canvas API.
    /// </summary>
    /// <param name="a">Horizontal scaling.</param>
    /// <param name="b">Horizontal skewing.</param>
    /// <param name="c">Vertical skewing.</param>
    /// <param name="d">Vertical scaling.</param>
    /// <param name="e">Horizontal translation.</param>
    /// <param name="f">Vertical translation.</param>
    /// <returns>The current <see cref="ICanvasPattern"/> instance for chaining.</returns>
    /// <remarks>
    /// This method corresponds to <c>CanvasPattern.setTransform()</c> with a 2D matrix in the HTML5 Canvas API.
    /// </remarks>
    ICanvasPattern SetTransform(
        double a = 1,
        double b = 0,
        double c = 0,
        double d = 1,
        double e = 0,
        double f = 0
    );

    /// <summary>
    /// Sets the transformation matrix that will be applied to the pattern when rendering.
    /// This overload uses a full 4x4 matrix, matching the <c>DOMMatrix</c> form in the HTML5 Canvas API for advanced transformations.
    /// </summary>
    /// <param name="m11">Component in the first row, first column.</param>
    /// <param name="m12">Component in the first row, second column.</param>
    /// <param name="m13">Component in the first row, third column.</param>
    /// <param name="m14">Component in the first row, fourth column.</param>
    /// <param name="m21">Component in the second row, first column.</param>
    /// <param name="m22">Component in the second row, second column.</param>
    /// <param name="m23">Component in the second row, third column.</param>
    /// <param name="m24">Component in the second row, fourth column.</param>
    /// <param name="m31">Component in the third row, first column.</param>
    /// <param name="m32">Component in the third row, second column.</param>
    /// <param name="m33">Component in the third row, third column.</param>
    /// <param name="m34">Component in the third row, fourth column.</param>
    /// <param name="m41">Component in the fourth row, first column.</param>
    /// <param name="m42">Component in the fourth row, second column.</param>
    /// <param name="m43">Component in the fourth row, third column.</param>
    /// <param name="m44">Component in the fourth row, fourth column.</param>
    /// <returns>The current <see cref="ICanvasPattern"/> instance for chaining.</returns>
    /// <remarks>
    /// This method corresponds to <c>CanvasPattern.setTransform()</c> with a 4x4 matrix in the HTML5 Canvas API.
    /// </remarks>
    ICanvasPattern SetTransform(
        double m11,
        double m12,
        double m13,
        double m14,
        double m21,
        double m22,
        double m23,
        double m24,
        double m31,
        double m32,
        double m33,
        double m34,
        double m41,
        double m42,
        double m43,
        double m44
    );
}
