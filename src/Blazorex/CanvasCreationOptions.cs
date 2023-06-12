using System;

namespace Blazorex
{
    public readonly struct CanvasCreationOptions
    {
        public bool Hidden { get; init; }
        public int Width { get; init; }
        public int Heigth { get; init; }
        public Action<IRenderContext> OnCanvasReady { get; init; }
        public Action<float> OnFrameReady { get; init; }

        public Action<int> OnKeyUp { get; init; }
        public Action<int> OnKeyDown { get; init; }
        public Action<MouseCoords> OnMouseMove { get; init; }
    }
}