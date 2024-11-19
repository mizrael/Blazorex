using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazorex
{
    internal class RenderContext2D : IRenderContext
    {
        private readonly string _id;
        private readonly IJSRuntime _jsRuntime;
        private readonly IJSInProcessRuntime _inProcessJsRuntime;

        private readonly Queue<JsOp> _jsOps = new();

        public RenderContext2D(string id, IJSRuntime jsRuntime)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
            }

            _id = id;
            _jsRuntime = jsRuntime;
            _inProcessJsRuntime = jsRuntime as IJSInProcessRuntime;
        }

        #region private methods

        private void Call(string method, params object[] args)
            => _jsOps.Enqueue(JsOp.FunctionCall(method, args));

        private void SetProperty(string property, object value)
            => _jsOps.Enqueue(JsOp.PropertyCall(property, value));

        private async ValueTask InvokeVoid(string identifier, params object?[]? args)
        {
            if(_inProcessJsRuntime is not null)
                _inProcessJsRuntime.InvokeVoid(identifier, args);
            else
                await _jsRuntime.InvokeVoidAsync(identifier, args);
        }

        private ValueTask<T> Invoke<T>(string identifier, params object?[]? args)
        {
            if (_inProcessJsRuntime is not null)
                return ValueTask.FromResult(_inProcessJsRuntime.Invoke<T>(identifier, args));
            else
                return _jsRuntime.InvokeAsync<T>(identifier, args);
        }

        #endregion private methods

        #region public methods

        async ValueTask IRenderContext.ProcessBatchAsync()
        {
            var payload = JsonSerializer.Serialize(_jsOps);
            _jsOps.Clear();

            await InvokeVoid("Blazorex.processBatch", _id, payload);
        }

        internal ValueTask<T> DirectCall<T>(string method, params object[] args)
        {
            var payload = string.Empty;
            if (args is not null && args.Length != 0)
                payload = JsonSerializer.Serialize(args);
            var result = Invoke<T>("Blazorex.directCall", _id, method, payload);
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

        public ValueTask<int> CreateImageDataAsync(int width, int height)
            => Invoke<int>("Blazorex.createImageData", _id, width, height);

        public void PutImageData(int imageDataId, byte[] data, double x, double y)
            => InvokeVoid("Blazorex.putImageData", _id, imageDataId, data, x, y);

        public ValueTask<TextMetrics> MeasureText(string text)
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

        public void LineTo(float x, float y)
            => this.Call("lineTo", x, y);

        public void MoveTo(float x, float y)
            => this.Call("moveTo", x, y);
        
        public void ArcTo(float x1, float y1, float x2, float y2, float radius)
            => this.Call("arcTo", x1, y1, x2, y2, radius);

        public void ClosePath()
            => this.Call("closePath");

        public void Fill()
            => this.Call("fill");

        public void Stroke()
            => this.Call("stroke");

        public void Arc(float x, float y, float radius, float startAngle, float endAngle, bool anticlockwise = false)
            => this.Call("arc", x, y, radius, startAngle, endAngle, anticlockwise);

        public void Rect(float x, float y, float width, float height)
            => this.Call("rect", x, y, width, height);

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