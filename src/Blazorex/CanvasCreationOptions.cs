using System;

namespace Blazorex
{
    public struct CanvasCreationOptions
    {
        public bool Hidden { get; init; }
        public int Width { get; init; }
        public int Heigth { get; init; }
        public Action<IRenderContext> OnCanvasReady { get; init; }

        public CanvasCreationOptions(bool hidden, int width, int heigth, Action<IRenderContext> onCanvasReady)
        {
            Hidden = hidden;
            Width = width;
            Heigth = heigth;
            OnCanvasReady = onCanvasReady;
        }
    }
}