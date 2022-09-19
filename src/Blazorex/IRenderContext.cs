using Microsoft.AspNetCore.Components;

namespace Blazorex
{
    public interface IRenderContext
    {
        void ClearRect(int x, int y, int width, int height);
        void DrawImage(ElementReference elementReference, double x, double y);
        void FillRect(int x, int y, int width, int height);
        void SetFillStyle(string value);
    }
}