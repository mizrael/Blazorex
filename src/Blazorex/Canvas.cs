using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazorex
{

    public class Canvas : ComponentBase
    {
        private readonly string _id = Guid.NewGuid().ToString();
        private RenderContext2D _context;     

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;
            builder.OpenElement(seq++, "canvas");
            builder.AddAttribute(seq++, "id", _id);
            builder.AddAttribute(seq++, "width", this.Width);
            builder.AddAttribute(seq++, "height", this.Height);

            builder.CloseElement();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await JSRuntime.InvokeVoidAsync("import", "./_content/Blazorex/blazorex.js");

            var managedInstance = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("Blazorex.initCanvas", _id, managedInstance);

            _context = new RenderContext2D(_id, this.JSRuntime as IJSUnmarshalledRuntime);
                        
            await this.OnCanvasReady.InvokeAsync(_context);
        }

        #region js interop

        [JSInvokable]
        public async ValueTask UpdateFrame(float timeStamp)
        {
            await this.OnFrameReady.InvokeAsync(timeStamp);
            _context.ProcessBatch();
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

        #endregion js interop

        #region Event Callbacks

        [Parameter]
        public EventCallback<int> OnKeyUp { get; set; }

        [Parameter]
        public EventCallback<int> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback<MouseCoords> OnMouseMove { get; set; }

        [Parameter]
        public EventCallback<float> OnFrameReady { get; set; }

        [Parameter]
        public EventCallback<IRenderContext> OnCanvasReady { get; set; }

        #endregion Event Callbacks

        #region properties

        [Inject]
        internal IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public int Width { get; set; } = 800;

        [Parameter]
        public int Height { get; set; } = 600;

        #endregion properties
    }

    public readonly struct MouseCoords
    {
        public readonly int X;
        public readonly int Y;
    }
}