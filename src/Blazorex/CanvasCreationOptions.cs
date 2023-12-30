using Microsoft.AspNetCore.Components;
using System;

namespace Blazorex
{
    public readonly struct CanvasCreationOptions
    {
        public ElementReference ElementReference { get; init; }
        public required int Width { get; init; }
        public required int Height { get; init; }

        public bool Hidden { get; init; }

        public Action<CanvasBase> OnCanvasReady { get; init; }
        public Action<float> OnFrameReady { get; init; }

        public Action<int> OnKeyUp { get; init; }
        public Action<int> OnKeyDown { get; init; }
        public Action<MouseCoords> OnMouseMove { get; init; }
        public Action<Size> OnResize { get; init; }
    }
}