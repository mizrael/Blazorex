using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    internal async ValueTask<T> DirectCall<T>(string method, params object[] args) =>
        await _blazorexAPI.InvokeAsync<T>("directCall", _id, method, args);

    async ValueTask IRenderContext.ProcessBatchAsync()
    {
        await _blazorexAPI.InvokeVoidAsync("processBatch", _id, _jsOps);
        _jsOps.Clear();
    }
}
