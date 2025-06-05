using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    internal ValueTask<T> DirectCall<T>(string method, params object[] args)
    {
        var payload = string.Empty;
        if (args is not null && args.Length != 0)
            payload = JsonSerializer.Serialize(args);
        var result = Invoke<T>("Blazorex.directCall", _id, method, payload);
        return result;
    }

    private async ValueTask InvokeVoid(string identifier, params object[] args)
    {
        if (_inProcessJsRuntime is not null)
            _inProcessJsRuntime.InvokeVoid(identifier, args);
        else
            await _jsRuntime.InvokeVoidAsync(identifier, args);
    }

    private ValueTask<T> Invoke<T>(string identifier, params object[] args)
    {
        if (_inProcessJsRuntime is not null)
            return ValueTask.FromResult(_inProcessJsRuntime.Invoke<T>(identifier, args));
        else
            return _jsRuntime.InvokeAsync<T>(identifier, args);
    }

    async ValueTask IRenderContext.ProcessBatchAsync()
    {
        var payload = JsonSerializer.Serialize(_jsOps);
        _jsOps.Clear();

        await InvokeVoid("Blazorex.processBatch", _id, payload);
    }
}
