using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Blazorex
{
    internal class RenderContext2D : IRenderContext
    {
        private readonly string _id;
        private readonly IJSUnmarshalledRuntime _jsRuntime;

        public RenderContext2D(string id, IJSUnmarshalledRuntime jsRuntime)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
            }

            _id = id;
            _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        }        

        private void Call(string method, params object[] args)
        {
            var jsonArgs = JsonSerializer.Serialize(args);

            _jsRuntime.InvokeUnmarshalled<string, string, string, object>("Blazorex.callCanvasMethod", _id, method, jsonArgs);
        }

        private void SetProperty(string property, object value)
        {
            string strVal = (value != null) ? value.ToString() : string.Empty;

            _jsRuntime.InvokeUnmarshalled<string, string, string, object>("Blazorex.setCanvasProperty", _id, property, strVal);
        }

        public void ClearRect(int x, int y, int width, int height)
        {
            this.Call("clearRect", x, y, width, height);
        }

        public void DrawImage(ElementReference imageRef, double x, double y)
        {
            var innerRef = new { id = imageRef.Id, isRef = true };
            this.Call("drawImage", innerRef, x, y);
        }

        public void FillRect(int x, int y, int width, int height)
        {
            this.Call("fillRect", x, y, width, height);
        }

        public void SetFillStyle(string value)
        {
            this.SetProperty("fillStyle", value);
        }

    }
}