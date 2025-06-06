using System;
using System.Collections.Generic;
using Blazorex.Interop;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    private readonly string _id;
    private readonly IJSObjectReference _blazorexAPI;
    private readonly MarshalReferencePool _marshalReferenceCache;
    private readonly Queue<JsOp> _jsOps = new();

    public RenderContext2D(string id, IJSObjectReference blazorexAPI)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException(
                $"'{nameof(id)}' cannot be null or whitespace.",
                nameof(id)
            );
        }

        this._id = id;
        this._blazorexAPI = blazorexAPI;
        this._marshalReferenceCache = new MarshalReferencePool();
    }

    internal void Call(string method, params object[] args) =>
        this._jsOps.Enqueue(JsOp.FunctionCall(method, args));

    private void SetProperty(string property, object value) =>
        this._jsOps.Enqueue(JsOp.PropertyCall(property, value));
}
