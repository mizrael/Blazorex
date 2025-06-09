using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorex;

/// <summary>
/// Defines a 2D rendering context for drawing shapes, text, images, and paths, similar to the HTML5 Canvas API.
/// </summary>
public interface IRenderContext
{
    /// <summary>
    /// Processes any pending drawing operations in the current batch asynchronously.
    /// </summary>
    internal ValueTask ProcessBatchAsync();

    /// <summary>
    /// Clears the specified rectangular area, making it fully transparent.
    /// </summary>
    /// <param name="x">The x-axis coordinate of the rectangle's starting point.</param>
    /// <param name="y">The y-axis coordinate of the rectangle's starting point.</param>
    /// <param name="width">The rectangle's width.</param>
    /// <param name="height">The rectangle's height.</param>
    void ClearRect(float x, float y, float width, float height);

    /// <summary>
    /// Fills the specified rectangle using the current <see cref="FillStyle"/>.
    /// </summary>
    void FillRect(float x, float y, float width, float height);

    /// <summary>
    /// Strokes the outline of the specified rectangle using the current <see cref="StrokeStyle"/> and <see cref="LineWidth"/>.
    /// </summary>
    void StrokeRect(float x, float y, float width, float height);

    /// <summary>
    /// Draws an image at the specified coordinates.
    /// </summary>
    /// <param name="imageRef">A reference to the image element.</param>
    /// <param name="x">The x-axis coordinate where to place the image.</param>
    /// <param name="y">The y-axis coordinate where to place the image.</param>
    void DrawImage(ElementReference imageRef, float x, float y);

    /// <summary>
    /// Draws an image at the specified coordinates, scaling it to the given width and height.
    /// </summary>
    void DrawImage(
        ElementReference imageRef,
        float x,
        float y,
        float imageWidth,
        float imageHeight
    );

    /// <summary>
    /// Draws a sub-rectangle of the source image to the specified destination rectangle.
    /// </summary>
    void DrawImage(
        ElementReference imageRef,
        float sourceX,
        float sourceY,
        float sourceWidth,
        float sourceHeight,
        float destX,
        float destY,
        float destWidth,
        float destHeight
    );

    /// <summary>
    /// Strokes (outlines) the given text at the specified position.
    /// </summary>
    void StrokeText(string text, float x, float y);

    /// <summary>
    /// Strokes (outlines) the given text at the specified position.
    /// </summary>
    void StrokeText(string text, float x, float y, float maxWidth);

    /// <summary>
    /// Fills the given text at the specified position using the current <see cref="FillStyle"/>.
    /// </summary>
    void FillText(string text, float x, float y);

    /// <summary>
    /// Fills the given text at the specified position using the current <see cref="FillStyle"/>.
    /// </summary>
    void FillText(string text, float x, float y, float maxWidth);

    /// <summary>
    /// Measures the dimensions of the given text using the current font settings.
    /// </summary>
    ValueTask<TextMetrics> MeasureText(string text);

    /// <summary>
    /// Creates a new blank image data object with the specified dimensions.
    /// </summary>
    ValueTask<int> CreateImageDataAsync(int width, int height);

    /// <summary>
    /// Puts the image data onto the canvas at the specified coordinates.
    /// </summary>
    void PutImageData(int imageDataId, byte[] data, double x, double y);

    /// <summary>
    /// Starts a new path by resetting the current path.
    /// </summary>
    void BeginPath();

    /// <summary>
    /// Creates a pattern using the specified image and repeat mode for filling shapes.
    /// </summary>
    ICanvasPattern CreatePattern(ElementReference imageRef, RepeatPattern pattern);

    /// <summary>
    /// Sets the transformation matrix for the context.
    /// </summary>
    void SetTransform(float a, float b, float c, float d, float e, float f);

    /// <summary>
    /// The CanvasRenderingContext2D.transform() method of the Canvas 2D API multiplies the current transformation with the matrix described by the arguments of this method. This lets you scale, rotate, translate (move), and skew the context.
    /// </summary>
    void Transform(float a, float b, float c, float d, float e, float f);

    /// <summary>
    /// Moves the origin of the context to the specified coordinates.
    /// </summary>
    void Translate(float x, float y);

    /// <summary>
    /// Rotates the context by the specified angle (in radians).
    /// </summary>
    void Rotate(float angle);

    /// <summary>
    /// Scales the context by the specified factors horizontally and vertically.
    /// </summary>
    void Scale(float x, float y);

    /// <summary>
    /// Saves the current drawing state onto the state stack.
    /// </summary>
    void Save();

    /// <summary>
    /// Restores the most recently saved drawing state from the stack.
    /// </summary>
    void Restore();

    /// <summary>
    /// Creates an arc/curve (part of a circle) with the given center, radius, and angles.
    /// </summary>
    void Arc(
        float x,
        float y,
        float radius,
        float startAngle,
        float endAngle,
        bool anticlockwise = false
    );

    /// <summary>
    /// Creates an arc with the given control points and radius, connecting two tangents.
    /// </summary>
    void ArcTo(float x1, float y1, float x2, float y2, float radius);

    /// <summary>
    /// Connects the last point in the current path to the specified coordinates with a straight line.
    /// </summary>
    void LineTo(float x, float y);

    /// <summary>
    /// Moves the starting point of a new sub-path to the specified coordinates.
    /// </summary>
    void MoveTo(float x, float y);

    /// <summary>
    /// Closes the current path by drawing a straight line back to the starting point.
    /// </summary>
    void ClosePath();

    /// <summary>
    /// Fills the current or given path with the current <see cref="FillStyle"/>.
    /// </summary>
    void Fill();

    /// <summary>
    /// Strokes the current or given path with the current <see cref="StrokeStyle"/>.
    /// </summary>
    void Stroke();

    /// <summary>
    /// Creates a rectangle path at the specified position with the given dimensions.
    /// </summary>
    void Rect(float x, float y, float width, float height);

    /// <summary>
    /// Creates a rounded rectangle path at the specified position with the given dimensions and corner radii.
    /// </summary>
    void RoundRect(float x, float y, float width, float height, params float[] radii);

    /// <summary>
    /// Gets or sets the style used for filling shapes (color, gradient, or pattern).
    /// </summary>
    object FillStyle { get; set; }

    /// <summary>
    /// Gets or sets the color or style used for strokes (outlines).
    /// </summary>
    string StrokeStyle { get; set; }

    /// <summary>
    /// Gets or sets the width of lines for strokes.
    /// </summary>
    int LineWidth { get; set; }

    /// <summary>
    /// Gets or sets the current font settings for text rendering.
    /// </summary>
    string Font { get; set; }

    /// <summary>
    /// Gets or sets the filter effect applied to drawing operations.
    /// </summary>
    Filter Filter { get; set; }

    /// <summary>
    /// Gets or sets the global alpha (opacity) value applied to all drawing.
    /// </summary>
    float GlobalAlpha { get; set; }

    /// <summary>
    /// Gets or sets the global composite operation used to determine how new drawings are combined with existing canvas content.
    /// </summary>
    GlobalCompositeOperation GlobalCompositeOperation { get; set; }

    /// <summary>
    /// Gets or sets the quality of image smoothing when scaling images.
    /// </summary>
    ImageSmoothingQuality ImageSmoothingQuality { get; set; }

    /// <summary>
    /// Gets or sets whether image smoothing is enabled when scaling images.
    /// </summary>
    bool ImageSmoothingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the current text alignment when drawing text.
    /// </summary>
    TextAlign TextAlign { get; set; }

    /// <summary>
    /// Gets or sets the current text baseline alignment when drawing text.
    /// </summary>
    TextBaseline TextBaseline { get; set; }

    /// <summary>
    /// Gets or sets the text rendering mode for drawing text.
    /// </summary>
    TextRendering TextRendering { get; set; }

    /// <summary>
    /// Sets the pattern of dashes and gaps used when stroking lines.
    /// </summary>
    void SetLineDash(params float[] segments);

    /// <summary>
    /// Resizes the canvas to the specified width and height.
    /// </summary>
    void Resize(int width, int height);

    /// <summary>
    /// Resets the rendering context to its default state, allowing it to be reused for drawing something else without having to explicitly reset all the properties.
    /// </summary>
    void Reset();

    /// <summary>
    /// Resets the current transform to the identity matrix.
    /// </summary>
    void ResetTransform();

    /// <summary>
    /// ShadowColor property of the canvas specifies the color of shadows.
    /// </summary>
    string ShadowColor { get; set; }

    /// <summary>
    /// ShadowOffsetX property of the canvas specifies the distance that shadows will be offset horizontally.
    /// </summary>
    float ShadowOffsetX { get; set; }

    /// <summary>
    /// ShadowOffsetY property of the canvas specifies the distance that shadows will be offset vertically.
    /// </summary>
    float ShadowOffsetY { get; set; }

    /// <summary>
    /// ShadowBlur property of the canvasI specifies the amount of blur applied to shadows. The default is 0 (no blur).
    /// </summary>
    float ShadowBlur { get; set; }

    /// <summary>
    /// Creates a linear gradient along the line connecting two given coordinates.
    /// The gradient can be used as a fill or stroke style.
    /// </summary>
    /// <param name="x0">The x-axis coordinate of the start point of the gradient.</param>
    /// <param name="y0">The y-axis coordinate of the start point of the gradient.</param>
    /// <param name="x1">The x-axis coordinate of the end point of the gradient.</param>
    /// <param name="y1">The y-axis coordinate of the end point of the gradient.</param>
    /// <returns>An <see cref="ICanvasGradient"/> object representing the linear gradient.</returns>
    ICanvasGradient CreateLinearGradient(float x0, float y0, float x1, float y1);

    /// <summary>
    /// Creates a conic gradient centered at the specified position, starting at the given angle.
    /// The gradient can be used as a fill or stroke style.
    /// </summary>
    /// <param name="startAngle">The angle in radians where the gradient begins, measured from the positive x-axis.</param>
    /// <param name="x">The x-axis coordinate of the center of the gradient.</param>
    /// <param name="y">The y-axis coordinate of the center of the gradient.</param>
    /// <returns>An <see cref="ICanvasGradient"/> object representing the conic gradient.</returns>
    ICanvasGradient CreateConicGradient(float startAngle, float x, float y);

    /// <summary>
    /// Creates a radial gradient along the line connecting two circles, defined by their centers and radii.
    /// The gradient can be used as a fill or stroke style.
    /// </summary>
    /// <param name="x0">The x-axis coordinate of the center of the starting circle.</param>
    /// <param name="y0">The y-axis coordinate of the center of the starting circle.</param>
    /// <param name="r0">The radius of the starting circle.</param>
    /// <param name="x1">The x-axis coordinate of the center of the ending circle.</param>
    /// <param name="y1">The y-axis coordinate of the center of the ending circle.</param>
    /// <param name="r1">The radius of the ending circle.</param>
    /// <returns>An <see cref="ICanvasGradient"/> object representing the radial gradient.</returns>
    ICanvasGradient CreateRadialGradient(
        float x0,
        float y0,
        float r0,
        float x1,
        float y1,
        float r1
    );
}
