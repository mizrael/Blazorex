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

            var context = new RenderContext2D(_id, this.JSRuntime as IJSUnmarshalledRuntime);

            await this.OnCanvasReady.InvokeAsync(context);
        }

        [JSInvokable()]
        public async ValueTask __BlazorexGameLoop(float timeStamp)
        {
            await this.OnFrameReady.InvokeAsync(timeStamp);
        }

        [Parameter]
        public EventCallback<float> OnFrameReady { get; set; }

        [Parameter]
        public EventCallback<IRenderContext> OnCanvasReady { get; set; }

        #region properties

        [Inject]
        internal IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public int Width { get; set; } = 800;

        [Parameter]
        public int Height { get; set; } = 600;

        #endregion properties
    }
}