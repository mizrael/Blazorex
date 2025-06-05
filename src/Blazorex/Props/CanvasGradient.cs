using Blazorex.Interop;
using Blazorex.Renderer;

namespace Blazorex;

internal sealed record CanvasGradient : CanvasProperty<int>, ICanvasGradient
{
    private readonly RenderContext2D _context;
    private readonly MarshalReference _marshalReference;

    internal CanvasGradient(RenderContext2D context, MarshalReference marshalReference)
        : base(marshalReference.Id)
    {
        _context = context;
        _marshalReference = marshalReference;
    }

    public ICanvasGradient AddColorStop(float offset, string color)
    {
        _context.Call("addColorStop", [_marshalReference, offset, color]);

        return this;
    }
}
