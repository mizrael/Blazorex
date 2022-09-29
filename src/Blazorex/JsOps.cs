using System.Text.Json.Serialization;

namespace Blazorex
{
    internal readonly struct JsOps
    {
        [JsonInclude]
        public readonly bool IsProperty;

        [JsonInclude] 
        public readonly string MethodName;

        [JsonInclude]
        public readonly object Args;

        public JsOps(string methodName, object args, bool isProperty) : this()
        {
            MethodName = methodName;
            Args = args;
            IsProperty = isProperty;
        }        
    }
}