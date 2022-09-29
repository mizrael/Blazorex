using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Blazorex
{
    internal class RenderContext2D : IRenderContext
    {
        private readonly string _id;
        private readonly IJSUnmarshalledRuntime _jsRuntime;

        private readonly Queue<JsOps> _jsOps = new();

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
            => _jsOps.Enqueue(new JsOps(method, args, false));

        private void SetProperty(string property, object value)
            => _jsOps.Enqueue(new JsOps(property, value, true));

        #endregion private methods

        #region public methods

        internal void ProcessBatch()
        {
            var payload = JsonSerializer.Serialize(_jsOps);
            _jsOps.Clear();
            _jsRuntime.InvokeUnmarshalled<string, string, object>("Blazorex.processBatch", _id, payload);
        }

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

        public void StrokeText(string text, double x, double y, double? maxWidth = null)
        {
            if (maxWidth.HasValue)
                this.Call("strokeText", text, x, y, maxWidth.Value);
            else
                this.Call("strokeText", text, x, y);
        }

        public void FillText(string text, double x, double y, double? maxWidth = null)
        {
            if (maxWidth.HasValue)
                this.Call("fillText", text, x, y, maxWidth.Value);
            else
                this.Call("fillText", text, x, y);
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

        private string _font;
        public string Font
        {
            get => _font;
            set
            {
                _font = value;
                this.SetProperty("font", value);
            }
        }


        #endregion properties
    }
}