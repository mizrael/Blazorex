using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorex;

public readonly struct CanvasCreationOptions
{
    /// <summary>
    /// The underlying DOM element reference for the canvas.
    /// </summary>
    public ElementReference ElementReference { get; init; }

    /// <summary>
    /// the width of the canvas
    /// </summary>
    public required int Width { get; init; }

    /// <summary>
    /// the height of the canvas
    /// </summary>
    public required int Height { get; init; }

    /// <summary>
    /// when true, the canvas will not be rendered, but will keep triggering events
    /// </summary>
    public bool Hidden { get; init; }

    /// <summary>
    /// A boolean value that indicates if the canvas contains an alpha channel.If set to false, the browser now knows that the backdrop is always opaque, which can speed up drawing of transparent content and images.
    /// </summary>
    public bool Alpha { get; init; }

    /// <summary>
    /// Specifies the color space of the rendering context. Possible values are:
    ///     "srgb" selects the sRGB color space.This is the default value.
    ///     "display-p3" selects the display-p3 color space.
    /// </summary>
    public ColorSpace ColorSpace { get; init; }

    /// <summary>
    /// A boolean value that hints the user agent to reduce the latency by desynchronizing the canvas paint cycle from the event loop.
    /// </summary>
    public bool Desynchronized { get; init; }

    /// <summary>
    /// A boolean value that indicates whether or not a lot of read-back operations are planned. This will force the use of a software (instead of hardware accelerated) 2D canvas and can save memory when calling GetImageData() frequently.
    /// </summary>
    public bool WillReadFrequently { get; init; }

    /// <summary>
    /// fired when the canvas is ready to process events
    /// </summary>
    public Action<CanvasBase> OnCanvasReady { get; init; }

    /// <summary>
    /// async version of <see cref="OnCanvasReady"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="OnCanvasReady"/> will ALWAYS take precedence over this, if both are set.
    /// </remarks>
    public Func<CanvasBase, ValueTask> OnCanvasReadyAsync { get; init; }

    /// <summary>
    /// fired at every frame refresh
    /// </summary>
    public Action<float> OnFrameReady { get; init; }

    /// <summary>
    /// Fired when a key is released while the canvas has focus.
    /// </summary>
    public Action<int> OnKeyUp { get; init; }

    /// <summary>
    /// Fired when a key is pressed while the canvas has focus.
    /// </summary>
    public Action<int> OnKeyDown { get; init; }

    /// <summary>
    /// Fired when the mouse moves over the canvas.
    /// </summary>
    public Action<MouseMoveEvent> OnMouseMove { get; init; }

    /// <summary>
    /// Fired when the mouse wheel is scrolled over the canvas.
    /// </summary>
    public Action<MouseScrollEvent> OnMouseWheel { get; init; }

    /// <summary>
    /// Fired when a mouse button is pressed down over the canvas.
    /// </summary>
    public Action<MouseClickEvent> OnMouseDown { get; init; }

    /// <summary>
    /// Fired when a mouse button is released over the canvas.
    /// </summary>
    public Action<MouseClickEvent> OnMouseUp { get; init; }

    /// <summary>
    /// Fired when the canvas is resized.
    /// </summary>
    public Action<Size> OnResize { get; init; }
}
