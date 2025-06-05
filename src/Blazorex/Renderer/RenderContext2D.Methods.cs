using System.Threading.Tasks;
using Blazorex.Interop;
using Microsoft.AspNetCore.Components;

namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    public void SetTransform(float a, float b, float c, float d, float e, float f) =>
        Call("setTransform", a, b, c, d, e, f);

    public void ClearRect(float x, float y, float width, float height) =>
        Call("clearRect", x, y, width, height);

    public void StrokeRect(float x, float y, float width, float height) =>
        Call("strokeRect", x, y, width, height);

    public void FillRect(float x, float y, float width, float height) =>
        Call("fillRect", x, y, width, height);

    public void DrawImage(ElementReference imageRef, float x, float y)
    {
        var innerRef = _marshalReferenceCache.Next(imageRef);

        Call("drawImage", innerRef, x, y);
    }

    public void DrawImage(
        ElementReference imageRef,
        float x,
        float y,
        float imageWidth,
        float imageHeight
    )
    {
        var innerRef = _marshalReferenceCache.Next(imageRef);
        Call("drawImage", innerRef, x, y, imageWidth, imageHeight);
    }

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
    )
    {
        var innerRef = _marshalReferenceCache.Next(imageRef);
        Call(
            "drawImage",
            innerRef,
            sourceX,
            sourceY,
            sourceWidth,
            sourceHeight,
            destX,
            destY,
            destWidth,
            destHeight
        );
    }

    public void StrokeText(string text, float x, float y, float? maxWidth = null)
    {
        if (maxWidth.HasValue)
            Call("strokeText", text, x, y, maxWidth.Value);
        else
            Call("strokeText", text, x, y);
    }

    public void FillText(string text, float x, float y, float? maxWidth = null)
    {
        if (maxWidth.HasValue)
            Call("fillText", text, x, y, maxWidth.Value);
        else
            Call("fillText", text, x, y);
    }

    public ValueTask<int> CreateImageDataAsync(int width, int height) =>
        Invoke<int>("Blazorex.createImageData", _id, width, height);

    public async void PutImageData(int imageDataId, byte[] data, double x, double y) =>
        await InvokeVoid("Blazorex.putImageData", _id, imageDataId, data, x, y);

    public ValueTask<TextMetrics> MeasureText(string text) =>
        DirectCall<TextMetrics>("measureText", text);

    public void Translate(float x, float y) => Call("translate", x, y);

    public void Rotate(float angle) => Call("rotate", angle);

    public void Scale(float x, float y) => Call("scale", x, y);

    public void BeginPath() => Call("beginPath");

    public void Save() => Call("save");

    public void Restore() => Call("restore");

    public void LineTo(float x, float y) => Call("lineTo", x, y);

    public void MoveTo(float x, float y) => Call("moveTo", x, y);

    public void ArcTo(float x1, float y1, float x2, float y2, float radius) =>
        Call("arcTo", x1, y1, x2, y2, radius);

    public void ClosePath() => Call("closePath");

    public void Fill() => Call("fill");

    public void Stroke() => Call("stroke");

    public void Arc(
        float x,
        float y,
        float radius,
        float startAngle,
        float endAngle,
        bool anticlockwise = false
    ) => Call("arc", x, y, radius, startAngle, endAngle, anticlockwise);

    public void Rect(float x, float y, float width, float height) =>
        Call("rect", x, y, width, height);

    public void RoundRect(float x, float y, float width, float height, params float[] radii) =>
        Call("roundRect", x, y, width, height, radii.Length == 1 ? radii[0] : radii);

    public void SetLineDash(params float[] segments) => Call("setLineDash", segments);

    public async void Resize(int width, int height) =>
        await InvokeVoid("Blazorex.resizeCanvas", _id, width, height);

    public ICanvasPattern CreatePattern(ElementReference imageRef, RepeatPattern pattern)
    {
        var marshalReference = _marshalReferenceCache.Next(imageRef);

        _jsOps.Enqueue(
            JsOp.FunctionCall("createPattern", new object[] { marshalReference, pattern.Value })
        );

        return new CanvasPattern(this, marshalReference);
    }

    public ICanvasGradient CreateLinearGradient(float x0, float y0, float x1, float y1)
    {
        var marshalReference = _marshalReferenceCache.Next(x0, y0, x1, y1);

        _jsOps.Enqueue(
            JsOp.FunctionCall(
                "createLinearGradient",
                new object[] { marshalReference, x0, y0, x1, y1 }
            )
        );

        return new CanvasGradient(this, marshalReference);
    }

    public ICanvasGradient CreateConicGradient(float startAngle, float x, float y)
    {
        var marshalReference = _marshalReferenceCache.Next(startAngle, x, y);

        _jsOps.Enqueue(
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
        var marshalReference = _marshalReferenceCache.Next(x0, y0, r0, x1, y1, r1);

        _jsOps.Enqueue(
            JsOp.FunctionCall(
                "createRadialGradient",
                new object[] { marshalReference, x0, y0, r0, x1, y1, r1 }
            )
        );

        return new CanvasGradient(this, marshalReference);
    }
}
