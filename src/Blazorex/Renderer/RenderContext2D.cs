using System;
using System.Collections.Generic;
using Blazorex.Interop;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    private readonly string _id;
    private readonly IJSRuntime _jsRuntime;
    private readonly IJSInProcessRuntime _inProcessJsRuntime;
    private readonly MarshalReferencePool _marshalReferenceCache;
    private readonly Queue<JsOp> _jsOps = new();

    public RenderContext2D(string id, IJSRuntime jsRuntime)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                $"'{nameof(id)}' cannot be null or whitespace.",
                nameof(id)
            );
        }

        _id = id;
        _jsRuntime = jsRuntime;
        _inProcessJsRuntime = jsRuntime as IJSInProcessRuntime;
        _marshalReferenceCache = new MarshalReferencePool();
    }

    internal void Call(string method, params object[] args) =>
        _jsOps.Enqueue(JsOp.FunctionCall(method, args));

    private void SetProperty(string property, object value) =>
        _jsOps.Enqueue(JsOp.PropertyCall(property, value));
}
