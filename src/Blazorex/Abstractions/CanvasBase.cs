using System;
using System.Threading;
using System.Threading.Tasks;
using Blazorex.Renderer;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorex;

public abstract class CanvasBase : ComponentBase, IAsyncDisposable
{
    private bool _disposed = false;
    private IJSObjectReference _module;
    private IJSObjectReference _blazorexAPI;

    protected override async Task OnInitializedAsync()
    {
        if (this.CanvasManager is null)
            return;

        await this.CanvasManager.OnChildCanvasAddedAsync(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        this._module = await this.JSRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            "./_content/Blazorex/blazorex.js"
        );

        this._blazorexAPI = await _module.InvokeAsync<IJSObjectReference>("createBlazorexAPI");

        var managedInstance = DotNetObjectReference.Create(this);

        await this._blazorexAPI.InvokeVoidAsync(
            "initCanvas",
            this.Id,
            managedInstance,
            new
            {
                alpha = this.Alpha,
                desynchronized = this.Desynchronized,
                colorSpace = this.ColorSpace != null
                    ? this.ColorSpace.Value
                    : ColorSpace.Srgb.Value,
                willReadFrequently = this.WillReadFrequently
            }
        );

        this.RenderContext = new RenderContext2D(this.Id, this._blazorexAPI);

        await this.OnCanvasReady.InvokeAsync(this);
    }

    #region JS interop

    [JSInvokable]
    public async ValueTask UpdateFrame(float timeStamp)
    {
        await this.OnFrameReady.InvokeAsync(timeStamp);
        await this.RenderContext.ProcessBatchAsync();
    }

    [JSInvokable]
    public async ValueTask Resized(int width, int height)
    {
        await this.OnResize.InvokeAsync(new Size(width, height));
    }

    [JSInvokable]
    public async ValueTask KeyPressed(KeyboardPressEvent keyCode)
    {
        await this.OnKeyDown.InvokeAsync(keyCode);
    }

    [JSInvokable]
    public async ValueTask KeyReleased(KeyboardPressEvent keyCode)
    {
        await this.OnKeyUp.InvokeAsync(keyCode);
    }

    [JSInvokable]
    public async ValueTask MouseMoved(MouseMoveEvent coordinates)
    {
        await this.OnMouseMove.InvokeAsync(coordinates);
    }

    [JSInvokable]
    public async ValueTask Wheel(MouseScrollEvent evt)
    {
        await this.OnMouseWheel.InvokeAsync(evt);
    }

    [JSInvokable]
    public async ValueTask MouseDown(MouseClickEvent evt)
    {
        await this.OnMouseDown.InvokeAsync(evt);
    }

    [JSInvokable]
    public async ValueTask MouseUp(MouseClickEvent evt)
    {
        await this.OnMouseUp.InvokeAsync(evt);
    }

    #endregion JS interop

    #region Event Callbacks

    [Parameter]
    public EventCallback<KeyboardPressEvent> OnKeyUp { get; set; }

    [Parameter]
    public EventCallback<KeyboardPressEvent> OnKeyDown { get; set; }

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
    ///     "sRGB" selects the sRGB color space.This is the default value.
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
        if (this.RenderContext == null)
            throw new InvalidOperationException(
                "Canvas not ready. Ensure OnCanvasReady has been called."
            );

        // Update the component properties
        this.Width = width;
        this.Height = height;

        // Trigger the resize operation
        this.RenderContext.Resize(width, height);
    }

    /// <summary>
    /// Converts canvas to blob.
    /// Docs: https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement/toBlob
    /// </summary>
    public async ValueTask<Blob> ToBlob(
        string type = "image/png",
        double? quality = null,
        CancellationToken cancellationToken = default
    )
    {
        var blobData = quality.HasValue
            ? await _blazorexAPI.InvokeAsync<BlobData>(
                "toBlob",
                cancellationToken,
                Id,
                type,
                quality.Value
            )
            : await _blazorexAPI.InvokeAsync<BlobData>("toBlob", cancellationToken, Id, type);

        return blobData.ToBlob();
    }

    /// <summary>
    /// Converts canvas to blob and invokes callback (fire-and-forget pattern).
    /// Docs: https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement/toBlob
    /// </summary>
    public void ToBlob(Action<Blob> callback, string type = "image/png", double? quality = null) =>
        _ = Task.Run(async () =>
        {
            try
            {
                using var blob = await ToBlob(type, quality);
                callback(blob);
            }
            catch { }
        });

    /// <summary>
    /// Converts canvas to data URL.
    /// Docs:https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement/toDataURL
    /// </summary>
    public async ValueTask<string> ToDataUrl(
        string type = "image/png",
        double? quality = null,
        CancellationToken cancellationToken = default
    )
    {
        using var blob = await ToBlob(type, quality, cancellationToken);
        return blob.ToDataUrl();
    }

    /// <summary>
    /// Converts canvas to object URL for direct browser usage.
    /// Docs: https://developer.mozilla.org/en-US/docs/Web/API/URL/createObjectURL_static
    /// </summary>
    public async ValueTask<string> CreateObjectURL(
        string type = "image/png",
        double? quality = null,
        CancellationToken cancellationToken = default
    )
    {
        using var blob = await ToBlob(type, quality, cancellationToken);
        return blob.ObjectUrl;
    }

    #endregion Public Methods

    #region Disposing

    public async ValueTask DisposeAsync()
    {
        if (!this._disposed)
        {
            if (this._blazorexAPI != null)
            {
                await this._blazorexAPI.InvokeVoidAsync("removeContext", Id);
                await this._blazorexAPI.DisposeAsync();
            }

            if (_module != null)
            {
                await this._module.DisposeAsync();
            }

            this._disposed = true;
        }
    }

    #endregion Disposing
}
