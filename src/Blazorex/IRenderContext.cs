using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazorex;

public interface IRenderContext
{
    internal ValueTask ProcessBatchAsync();

    void ClearRect(float x, float y, float width, float height);
    void FillRect(float x, float y, float width, float height);
    void StrokeRect(float x, float y, float width, float height);

    void DrawImage(ElementReference imageRef, float x, float y);
    void DrawImage(ElementReference imageRef, float x, float y, float imageWidth, float imageHeight);
    void DrawImage(ElementReference imageRef,
                  float sourceX, float sourceY, float sourceWidth, float sourceHeight,
                  float destX, float destY, float destWidth, float destHeight);

    void StrokeText(string text, float x, float y, float? maxWidth = null);
    void FillText(string text, float x, float y, float? maxWidth = null);
    ValueTask<TextMetrics> MeasureText(string text);

    ValueTask<int> CreateImageDataAsync(int width, int height);
    void PutImageData(int imageDataId, byte[] data, double x, double y);

    void BeginPath();

    object CreatePattern(ElementReference imageRef, RepeatPattern pattern);

    void SetTransform(float a, float b, float c, float d, float e, float f);
    void Translate(float x, float y);
    void Rotate(float angle);
    void Scale(float x, float y);

    void Save();
    void Restore();
    void Arc(float x, float y, float radius, float startAngle, float endAngle, bool anticlockwise = false);
    void ArcTo(float x1, float y1, float x2, float y2, float radius);
    void LineTo(float x, float y);
    void MoveTo(float x, float y);
    void ClosePath();
    void Fill();
    void Stroke();
    void Rect(float x, float y, float width, float height);


    object FillStyle { get; set; }
    string StrokeStyle { get; set; }
    int LineWidth { get; set; }
    string Font { get; set; }

    TextAlign TextAlign { get; set; }
    TextBaseline TextBaseline { get; set; }
    
    void SetLineDash(float[] segments);
    void Resize(int width, int height);
}
