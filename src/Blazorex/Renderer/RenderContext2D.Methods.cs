using System.Threading.Tasks;
using Blazorex.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    public void SetTransform(float a, float b, float c, float d, float e, float f) =>
        this.Call("setTransform", a, b, c, d, e, f);

    public void ClearRect(float x, float y, float width, float height) =>
        this.Call("clearRect", x, y, width, height);

    public void StrokeRect(float x, float y, float width, float height) =>
        this.Call("strokeRect", x, y, width, height);

    public void FillRect(float x, float y, float width, float height) =>
        this.Call("fillRect", x, y, width, height);

    public void DrawImage(ElementReference imageRef, float x, float y) =>
        this.Call("drawImage", _marshalReferenceCache.Next(imageRef), x, y);

    public void DrawImage(
        ElementReference imageRef,
        float x,
        float y,
        float imageWidth,
        float imageHeight
    ) =>
        this.Call(
            "drawImage",
            _marshalReferenceCache.Next(imageRef),
            x,
            y,
            imageWidth,
            imageHeight
        );

    public void DrawImage(
        ElementReference imageRef,
        float sourceX,
        float sourceY,
        float sourceWidth,
        float sourceHeight,
        float destX,
        float destY,
        float destWidth,
        float destHeight
    ) =>
        this.Call(
            "drawImage",
            _marshalReferenceCache.Next(imageRef),
            sourceX,
            sourceY,
            sourceWidth,
            sourceHeight,
            destX,
            destY,
            destWidth,
            destHeight
        );

    public void Reset() => this.Call("reset");

    public void ResetTransform() => this.Call("resetTransform");

    public void Transform(float a, float b, float c, float d, float e, float f) =>
        this.Call("transform", a, b, c, d, e, f);

    public void StrokeText(string text, float x, float y) => this.Call("strokeText", text, x, y);

    public void StrokeText(string text, float x, float y, float maxWidth) =>
        this.Call("strokeText", text, x, y, maxWidth);

    public void FillText(string text, float x, float y) => this.Call("fillText", text, x, y);

    public void FillText(string text, float x, float y, float maxWidth) =>
        this.Call("fillText", text, x, y, maxWidth);

    public ValueTask<int> CreateImageDataAsync(int width, int height) =>
        this._blazorexAPI.InvokeAsync<int>("createImageData", _id, width, height);

    public void PutImageData(int imageDataId, byte[] data, double x, double y) =>
        this._blazorexAPI.InvokeVoidAsync("putImageData", _id, imageDataId, data, x, y);

    public ValueTask<TextMetrics> MeasureText(string text) =>
        this.DirectCall<TextMetrics>("measureText", text);

    public void Translate(float x, float y) => this.Call("translate", x, y);

    public void Rotate(float angle) => this.Call("rotate", angle);

    public void Scale(float x, float y) => this.Call("scale", x, y);

    public void BeginPath() => this.Call("beginPath");

    public void Save() => this.Call("save");

    public void Restore() => this.Call("restore");

    public void LineTo(float x, float y) => this.Call("lineTo", x, y);

    public void MoveTo(float x, float y) => this.Call("moveTo", x, y);

    public void ArcTo(float x1, float y1, float x2, float y2, float radius) =>
        this.Call("arcTo", x1, y1, x2, y2, radius);

    public void ClosePath() => this.Call("closePath");

    public void Fill() => this.Call("fill");

    public void Stroke() => this.Call("stroke");

    public void Arc(
        float x,
        float y,
        float radius,
        float startAngle,
        float endAngle,
        bool anticlockwise = false
    ) => this.Call("arc", x, y, radius, startAngle, endAngle, anticlockwise);

    public void Rect(float x, float y, float width, float height) =>
        this.Call("rect", x, y, width, height);

    public void RoundRect(float x, float y, float width, float height, params float[] radii) =>
        this.Call("roundRect", x, y, width, height, radii.Length == 1 ? radii[0] : radii);

    public void SetLineDash(params float[] segments) => this.Call("setLineDash", segments);

    public async void Resize(int width, int height) =>
        await this._blazorexAPI.InvokeVoidAsync("resizeCanvas", _id, width, height);

    public ICanvasPattern CreatePattern(ElementReference imageRef, RepeatPattern pattern)
    {
        var marshalReference = this._marshalReferenceCache.Next(imageRef);

        this._jsOps.Enqueue(
            JsOp.FunctionCall("createPattern", new object[] { marshalReference, pattern.Value })
        );

        return new CanvasPattern(this, marshalReference);
    }

    public ICanvasGradient CreateLinearGradient(float x0, float y0, float x1, float y1)
    {
        var marshalReference = this._marshalReferenceCache.Next(x0, y0, x1, y1);

        this._jsOps.Enqueue(
            JsOp.FunctionCall(
                "createLinearGradient",
                new object[] { marshalReference, x0, y0, x1, y1 }
            )
        );

        return new CanvasGradient(this, marshalReference);
    }

    public ICanvasGradient CreateConicGradient(float startAngle, float x, float y)
    {
        var marshalReference = this._marshalReferenceCache.Next(startAngle, x, y);

        this._jsOps.Enqueue(
            JsOp.FunctionCall(
                "createConicGradient",
                new object[] { marshalReference, startAngle, x, y }
            )
        );

        return new CanvasGradient(this, marshalReference);
    }

    public ICanvasGradient CreateRadialGradient(
        float x0,
        float y0,
        float r0,
        float x1,
        float y1,
        float r1
    )
    {
        var marshalReference = this._marshalReferenceCache.Next(x0, y0, r0, x1, y1, r1);

        this._jsOps.Enqueue(
            JsOp.FunctionCall(
                "createRadialGradient",
                new object[] { marshalReference, x0, y0, r0, x1, y1, r1 }
            )
        );

        return new CanvasGradient(this, marshalReference);
    }
}
