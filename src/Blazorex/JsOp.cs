using System.Text.Json.Serialization;

namespace Blazorex
{
    internal readonly struct JsOp
    {
        [JsonInclude]
        public readonly bool IsProperty;

        [JsonInclude] 
        public readonly string MethodName;

        [JsonInclude]
        public readonly object Args;

        private JsOp(string methodName, object args, bool isProperty) : this()
        {
            MethodName = methodName;
            Args = args;
            IsProperty = isProperty;
        }        

        public static JsOp FunctionCall(string methodName, object args)
            => new(methodName, args, false);

        public static JsOp PropertyCall(string propertyName, object args)
            => new(propertyName, args, true);
    }
}