namespace Blazorex.Samples.KeyAndMouse.Pages;

public partial class Home
{
    private CanvasManager? _canvasManager;
    private IRenderContext? _context;

    private const int CanvasWidth = 1024;
    private const int CanvasHeight = 768;

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _canvasManager?.CreateCanvas(
            "keyrain",
            new CanvasCreationOptions
            {
                Hidden = false,
                Width = CanvasWidth,
                Height = CanvasHeight,
                Alpha = false,
                Desynchronized = true, // Better performance for animations
                WillReadFrequently = false,
                OnCanvasReady = OnCanvasReady,
                OnFrameReady = OnFrameReady,
                OnKeyDown = OnKeyPressed
            }
        );
    }

    private void OnCanvasReady(CanvasBase canvas)
    {
        _context = canvas.RenderContext;
    }

    private void OnFrameReady(float timestamp)
    {
        if (_context is null)
            return;

        KeyRainEngine.UpdateAndRender(_context);
    }

    private void OnKeyPressed(KeyboardPressEvent e)
    {
        if (_context is null)
            return;

        // Ignore modifier keys and special keys
        if (IsValidKey(e.Key))
        {
            KeyRainEngine.AddKey(e.Key);
        }
    }

    /// <summary>Determines if a key should create a falling character</summary>
    private static bool IsValidKey(string key) =>
        key.Length == 1
        && (char.IsLetterOrDigit(key[0]) || char.IsPunctuation(key[0]) || char.IsSymbol(key[0]));
}
