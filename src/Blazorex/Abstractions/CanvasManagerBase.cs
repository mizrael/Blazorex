using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorex.Abstractions;

public abstract class CanvasManagerBase : ComponentBase
{
    protected readonly Dictionary<string, CanvasCreationOptions> _names = new();
    protected readonly Dictionary<string, CanvasBase> _canvases = new();

    public void CreateCanvas(string name, CanvasCreationOptions options)
    {
        _names.Add(name, options);

        StateHasChanged();
    }

    internal async ValueTask OnChildCanvasAddedAsync(CanvasBase canvas)
    {
        await OnCanvasAdded.InvokeAsync(canvas);
    }

    [Parameter]
    public EventCallback<CanvasBase> OnCanvasAdded { get; set; }
}
