using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
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

        #region private methods

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

        #endregion private methods

        #region public methods

        public void ClearRect(int x, int y, int width, int height)
            => this.Call("clearRect", x, y, width, height);

        public void StrokeRect(double x, double y, double width, double height)
             => this.Call("strokeRect", x, y, width, height);

        public void FillRect(int x, int y, int width, int height)
            => this.Call("fillRect", x, y, width, height);

        public void DrawImage(ElementReference imageRef, double x, double y)
        {
            var innerRef = MarshalReference.Map(imageRef);

            this.Call("drawImage", innerRef, x, y);
        }

        public void DrawImage(ElementReference imageRef, double x, double y, int imageWidth, int imageHeight)
        {
            var innerRef = MarshalReference.Map(imageRef);

            this.Call("drawImage", innerRef, x, y, imageWidth, imageHeight);
        }

        #endregion public methods

        #region properties

        private string _fillStyle;
        public string FillStyle
        {
            get => _fillStyle;
            set
            {
                _fillStyle = value;
                this.SetProperty("fillStyle", value);
            }
        }

        private string _strokeStyle;
        public string StrokeStyle
        {
            get => _strokeStyle;
            set
            {
                _strokeStyle = value;
                this.SetProperty("strokeStyle", value);
            }
        }

        private int _lineWidth;
        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                this.SetProperty("lineWidth", value);
            }
        }

        #endregion properties
    }
}