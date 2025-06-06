using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    internal async ValueTask<T> DirectCall<T>(string method, params object[] args)
        => await _blazorexAPI.InvokeAsync<T>("directCall", _id, method, args);

    private ValueTask InvokeVoid(string identifier, params object[] args) =>
        _blazorexAPI.InvokeVoidAsync(identifier, args);

    private ValueTask<T> Invoke<T>(string identifier, params object[] args) =>
        _blazorexAPI.InvokeAsync<T>(identifier, args);

    async ValueTask IRenderContext.ProcessBatchAsync()
    {
        await _blazorexAPI.InvokeVoidAsync("processBatch", _id, _jsOps);
        _jsOps.Clear();
    }
}
