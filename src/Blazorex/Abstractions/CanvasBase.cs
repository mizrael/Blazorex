using System;
using System.Threading.Tasks;
using Blazorex.Renderer;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorex;

public abstract class CanvasBase : ComponentBase, IAsyncDisposable
{
    private bool _disposed = false;

    protected override async Task OnInitializedAsync()
    {
        if (CanvasManager is null)
            return;

        await CanvasManager.OnChildCanvasAddedAsync(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await JSRuntime.InvokeVoidAsync("import", "./_content/Blazorex/blazorex.js");

        var managedInstance = DotNetObjectReference.Create(this);

        await JSRuntime.InvokeVoidAsync(
            "Blazorex.initCanvas",
            Id,
            managedInstance,
            new
            {
                alpha = Alpha,
                desynchronized = Desynchronized,
                colorSpace = ColorSpace != null ? ColorSpace.Value : ColorSpace.Srgb.Value,
                willReadFrequently = WillReadFrequently
            }
        );

        RenderContext = new RenderContext2D(Id, JSRuntime);

        await OnCanvasReady.InvokeAsync(this);
    }

    #region JS interop

    [JSInvokable]
    public async ValueTask UpdateFrame(float timeStamp)
    {
        await OnFrameReady.InvokeAsync(timeStamp);
        await RenderContext.ProcessBatchAsync();
    }

    [JSInvokable]
    public async ValueTask Resized(int width, int height)
    {
        await OnResize.InvokeAsync(new Size(width, height));
    }

    [JSInvokable]
    public async ValueTask KeyPressed(int keyCode)
    {
        await OnKeyDown.InvokeAsync(keyCode);
    }

    [JSInvokable]
    public async ValueTask KeyReleased(int keyCode)
    {
        await OnKeyUp.InvokeAsync(keyCode);
    }

    [JSInvokable]
    public async ValueTask MouseMoved(MouseMoveEvent coords)
    {
        await OnMouseMove.InvokeAsync(coords);
    }

    [JSInvokable]
    public async ValueTask Wheel(MouseScrollEvent evt)
    {
        await OnMouseWheel.InvokeAsync(evt);
    }

    [JSInvokable]
    public async ValueTask MouseDown(MouseClickEvent evt)
    {
        await OnMouseDown.InvokeAsync(evt);
    }

    [JSInvokable]
    public async ValueTask MouseUp(MouseClickEvent evt)
    {
        await OnMouseUp.InvokeAsync(evt);
    }

    #endregion JS interop

    #region Event Callbacks

    [Parameter]
    public EventCallback<int> OnKeyUp { get; set; }

    [Parameter]
    public EventCallback<int> OnKeyDown { get; set; }

    [Parameter]
    public EventCallback<MouseMoveEvent> OnMouseMove { get; set; }

    [Parameter]
    public EventCallback<MouseScrollEvent> OnMouseWheel { get; set; }

    [Parameter]
    public EventCallback<MouseClickEvent> OnMouseDown { get; set; }

    [Parameter]
    public EventCallback<MouseClickEvent> OnMouseUp { get; set; }

    [Parameter]
    public EventCallback<Size> OnResize { get; set; }

    [Parameter]
    public EventCallback<float> OnFrameReady { get; set; }

    [Parameter]
    public EventCallback<CanvasBase> OnCanvasReady { get; set; }

    #endregion Event Callbacks

    #region Properties

    public string Id { get; } = Guid.NewGuid().ToString();

    [Inject]
    internal IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public int Width { get; set; } = 800;

    [Parameter]
    public int Height { get; set; } = 600;

    [Parameter]
    public string Name { get; set; }

    /// <summary>
    /// A boolean value that indicates if the canvas contains an alpha channel.If set to false, the browser now knows that the backdrop is always opaque, which can speed up drawing of transparent content and images.
    /// </summary>
    [Parameter]
    public bool Alpha { get; set; }

    /// <summary>
    /// Specifies the color space of the rendering context. Possible values are:
    ///     "srgb" selects the sRGB color space.This is the default value.
    ///     "display-p3" selects the display-p3 color space.
    /// </summary>
    [Parameter]
    public ColorSpace ColorSpace { get; set; }

    /// <summary>
    /// A boolean value that hints the user agent to reduce the latency by desynchronizing the canvas paint cycle from the event loop.
    /// </summary>
    [Parameter]
    public bool Desynchronized { get; set; } = true;

    /// <summary>
    /// A boolean value that indicates whether or not a lot of read-back operations are planned. This will force the use of a software (instead of hardware accelerated) 2D canvas and can save memory when calling GetImageData() frequently.
    /// </summary>
    [Parameter]
    public bool WillReadFrequently { get; set; }

    public ElementReference ElementReference { get; internal set; }

    [CascadingParameter]
    public CanvasManager CanvasManager { get; set; }

    public IRenderContext RenderContext { get; private set; }

    #endregion Properties

    #region Public Methods

    /// <summary>
    /// Resizes the canvas to the specified dimensions.
    /// Note: This will clear the canvas and reset the drawing context.
    /// </summary>
    /// <param name="width">New canvas width</param>
    /// <param name="height">New canvas height</param>
    public void Resize(int width, int height)
    {
        if (RenderContext == null)
            throw new InvalidOperationException(
                "Canvas not ready. Ensure OnCanvasReady has been called."
            );

        // Update the component properties
        Width = width;
        Height = height;

        // Trigger the resize operation
        RenderContext.Resize(width, height);
    }

    #endregion Public Methods

    #region Disposing

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            // Call javascript to delete this canvas Id from the _contexts array
            await JSRuntime.InvokeVoidAsync("Blazorex.removeContext", Id);

            _disposed = true;
        }
    }

    #endregion Disposing
}
