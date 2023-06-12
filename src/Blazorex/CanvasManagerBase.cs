using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazorex
{
    public abstract class CanvasManagerBase : ComponentBase
    {
        protected readonly Dictionary<string, CanvasCreationOptions> _names = new();
        protected readonly Dictionary<string, Canvas> _canvases = new();

        public async ValueTask<Canvas> CreateCanvas(
            string name, 
            CanvasCreationOptions options)
        {
            _names.Add(name, options);

            this.StateHasChanged();

            await this.OnCanvasAdded.InvokeAsync();

            return _canvases[name];
        }

        [Parameter]
        public EventCallback OnCanvasAdded { get; set; }
    }
}