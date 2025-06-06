using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Blazorex.Interop;

[StructLayout(LayoutKind.Auto)]
internal readonly record struct JsOp
{
    [JsonInclude]
    public readonly bool IsProperty;

    [JsonInclude]
    public readonly string MethodName;

    [JsonInclude]
    public readonly object Args;

    private JsOp(string methodName, object args, bool isProperty) =>
        (MethodName, Args, IsProperty) = (methodName, args, isProperty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsOp FunctionCall(string methodName, object args) => new(methodName, args, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static JsOp PropertyCall(string propertyName, object args) =>
        new(propertyName, args, true);
}
