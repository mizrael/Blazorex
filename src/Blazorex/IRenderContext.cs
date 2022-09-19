using Microsoft.AspNetCore.Components;

namespace Blazorex
{
    public interface IRenderContext
    {
        void ClearRect(int x, int y, int width, int height);
        void FillRect(int x, int y, int width, int height);
        void StrokeRect(double x, double y, double width, double height);

        void DrawImage(ElementReference elementReference, double x, double y);
        void DrawImage(ElementReference imageRef, double x, double y, int imageWidth, int imageHeight);
        

        string FillStyle { get; set; }
        string StrokeStyle { get; set; }
        int LineWidth { get; set; }
    }
}