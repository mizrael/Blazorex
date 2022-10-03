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

        internal T DirectCall<T>(string method, params object[] args)
        {
            var payload = string.Empty;
            if(args is not null && args.Length != 0)
                payload = JsonSerializer.Serialize(args);
            var result = _jsRuntime.InvokeUnmarshalled<string, string, string, T>("Blazorex.directCall", _id, method, payload);
            return result;
        }

        public void ClearRect(float x, float y, float width, float height)
            => this.Call("clearRect", x, y, width, height);

        public void StrokeRect(float x, float y, float width, float height)
             => this.Call("strokeRect", x, y, width, height);

        public void FillRect(float x, float y, float width, float height)
            => this.Call("fillRect", x, y, width, height);

        public object CreatePattern(ElementReference imageRef, RepeatPattern pattern)
        {
            var innerRef = MarshalReference.Map(imageRef);
            return DirectCall<object>("createPattern", innerRef, pattern.Value);
        }

        public void DrawImage(ElementReference imageRef, float x, float y)
        {
            var innerRef = MarshalReference.Map(imageRef);

            this.Call("drawImage", innerRef, x, y);
        }

        public void DrawImage(ElementReference imageRef, float x, float y, float imageWidth, float imageHeight)
        {
            var innerRef = MarshalReference.Map(imageRef);

            this.Call("drawImage", innerRef, x, y, imageWidth, imageHeight);
        }

        public void DrawImage(ElementReference imageRef,
              float sourceX, float sourceY, float sourceWidth, float sourceHeight,
              float destX, float destY, float destWidth, float destHeight)
        {
            var innerRef = MarshalReference.Map(imageRef);

            this.Call("drawImage", innerRef, sourceX, sourceY, sourceWidth, sourceHeight,
                                            destX, destY, destWidth, destHeight);
        }

        public void StrokeText(string text, float x, float y, float? maxWidth = null)
        {
            if (maxWidth.HasValue)
                this.Call("strokeText", text, x, y, maxWidth.Value);
            else
                this.Call("strokeText", text, x, y);
        }

        public void FillText(string text, float x, float y, float? maxWidth = null)
        {
            if (maxWidth.HasValue)
                this.Call("fillText", text, x, y, maxWidth.Value);
            else
                this.Call("fillText", text, x, y);
        }

        public TextMetrics MeasureText(string text)
            => DirectCall<TextMetrics>("measureText", text);

        public void Translate(float x, float y)
            => this.Call("translate", x, y);
        
        public void Rotate(float angle)
            => this.Call("rotate", angle);

        public void Scale(float x, float y)
            => this.Call("scale", x, y);

        public void BeginPath()
            => this.Call("beginPath");

        public void Save()
            => this.Call("save");
        public void Restore()
            => this.Call("restore");

        #endregion public methods

        #region properties

        private object _fillStyle;
        public object FillStyle
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

        private TextAlign _textAlign;
        public TextAlign TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                this.SetProperty("textAlign", value.Value);
            }
        }


        #endregion properties
    }
}