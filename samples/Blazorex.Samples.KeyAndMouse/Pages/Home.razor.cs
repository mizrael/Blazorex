namespace Blazorex.Samples.KeyAndMouse.Pages;

public partial class Home
{
    private CanvasManager? _canvasManager;
    private IRenderContext? _context;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _canvasManager?.CreateCanvas(
            "main",
            new CanvasCreationOptions()
            {
                Hidden = false,
                Width = KeyAndMouseEngine.W,
                Height = KeyAndMouseEngine.H,
                Alpha = false,
                Desynchronized = false,
                WillReadFrequently = true,
                OnCanvasReady = this.OnMainCanvasReady,
                OnFrameReady = this.OnMainFrameReady,
                OnMouseMove = (ev) =>
                {
                    if (_context is null)
                    {
                        return;
                    }

                    KeyAndMouseEngine.AddMouseTrail(ev.OffsetX, ev.OffsetY);
                },
                OnKeyDown = (ev) =>
                {
                    if (_context is null)
                    {
                        return;
                    }

                    KeyAndMouseEngine.PlaceAndMakeKeyCapFallDown(ev.Key);
                }
            }
        );
    }

    private void OnMainCanvasReady(CanvasBase canvas)
    {
        _context = canvas.RenderContext;
    }

    private void OnMainFrameReady(float timestamp)
    {
        if (_context != null)
        {
            // Clear the canvas if needed
            _context.FillStyle = "#fff";
            _context.FillRect(0, 0, 1024, 768);

            // Update and draw the mouse trail every frame
            KeyAndMouseEngine.UpdateAndDrawFallingKeyCaps(_context);
            KeyAndMouseEngine.UpdateAndDrawMouseTrail(_context);
        }
    }
}
