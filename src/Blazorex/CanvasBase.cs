using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazorex
{
    public class CanvasBase : ComponentBase, IAsyncDisposable
    {
        private bool _disposed = false;

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

            await JSRuntime.InvokeVoidAsync("import", "./_content/Blazorex/blazorex.js");

            var managedInstance = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("Blazorex.initCanvas", Id, managedInstance);

            this.RenderContext = new RenderContext2D(Id, this.JSRuntime as IJSInProcessRuntime);
                        
            await this.OnCanvasReady.InvokeAsync(this);
        }

        #region JS interop

        [JSInvokable]
        public async ValueTask UpdateFrame(float timeStamp)
        {
            await this.OnFrameReady.InvokeAsync(timeStamp);
            this.RenderContext.ProcessBatch();
        }

        [JSInvokable]
        public async ValueTask KeyPressed(int keyCode)
        {
            await this.OnKeyDown.InvokeAsync(keyCode);
        }

        [JSInvokable]
        public async ValueTask KeyReleased(int keyCode)
        {
            await this.OnKeyUp.InvokeAsync(keyCode);
        }

        [JSInvokable]
        public async ValueTask MouseMoved(MouseCoords coords)
        {
            await this.OnMouseMove.InvokeAsync(coords);
        }

        [JSInvokable]
        public async ValueTask Resized(int width, int height)
        {
            await this.OnResize.InvokeAsync(new Size(width, height));
        }

        #endregion JS interop

        #region Event Callbacks

        [Parameter]
        public EventCallback<int> OnKeyUp { get; set; }

        [Parameter]
        public EventCallback<int> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<MouseCoords> OnMouseMove { get; set; }

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

        public ElementReference ElementReference { get; internal set; }

        [CascadingParameter]
        public CanvasManager CanvasManager { get; set; }

        public IRenderContext RenderContext { get; private set; }

        #endregion Properties

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
}