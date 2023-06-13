using Microsoft.AspNetCore.Components;

namespace Blazorex
{
    public interface IRenderContext
    {
        internal void ProcessBatch();

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
        TextMetrics MeasureText(string text);

        int CreateImageData(int width, int height);
        void PutImageData(int imageDataId, byte[] data, double x, double y);

        void BeginPath();

        object CreatePattern(ElementReference imageRef, RepeatPattern pattern);

        void Translate(float x, float y);
        void Rotate(float angle);
        void Scale(float x, float y);

        void Save();
        void Restore();


        object FillStyle { get; set; }
        string StrokeStyle { get; set; }
        int LineWidth { get; set; }
        string Font { get; set; }

        TextAlign TextAlign { get; set; }
    }
}