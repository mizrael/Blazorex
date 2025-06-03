using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorex;

public readonly struct CanvasCreationOptions
{
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
    public Action<int> OnKeyUp { get; init; }
    public Action<int> OnKeyDown { get; init; }
    public Action<MouseCoords> OnMouseMove { get; init; }
    public Action<WheelDelta> OnMouseWheel { get; init; }
    public Action<MouseButtonData> OnMouseDown { get; init; }
    public Action<MouseButtonData> OnMouseUp { get; init; }
    public Action<Size> OnResize { get; init; }
}
