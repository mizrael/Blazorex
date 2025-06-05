using Microsoft.AspNetCore.Components;
using game = Blazorex.Samples.FallingBlocks.FallingBlocksGame;

namespace Blazorex.Samples.FallingBlocks.Pages;

public partial class Home
{
    private CanvasManager? _canvasManager;
    private IRenderContext? _context;
    private Func<ValueTask>? _focusAction;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _canvasManager?.CreateCanvas(
            "main",
            new CanvasCreationOptions()
            {
                Hidden = false,
                Width = game.W,
                Height = game.H,
                OnCanvasReady = this.OnMainCanvasReady,
                OnFrameReady = this.OnMainFrameReady,
                OnKeyUp = (key) =>
                {
                    if (_context == null || game.Lose)
                    {
                        return;
                    }

                    game.KeyUp(key);
                },
            }
        );
    }

    private void OnMainCanvasReady(CanvasBase canvas)
    {
        _context = canvas.RenderContext;
        _focusAction = () => canvas.ElementReference.FocusAsync(true);

        game.Lose = true;
    }

    private float _lastRenderTime = 0;
    private float _lastTickTime = 0;

    private void OnMainFrameReady(float timestamp)
    {
        if (_context == null || game.Lose)
        {
            return;
        }

        // Render every 30ms
        if (timestamp - _lastRenderTime >= 30f)
        {
            game.Render(_context);
            _lastRenderTime = timestamp;
        }

        // Tick every 400ms
        if (timestamp - _lastTickTime >= 400f)
        {
            game.Tick();
            _lastTickTime = timestamp;
        }
    }

    private async Task OnNewGame()
    {
        if (_context == null)
        {
            return;
        }

        _lastRenderTime = 0;
        _lastTickTime = 0;

        game.Init();
        game.NewShape();
        game.Lose = false;

        if (_focusAction is not null)
        {
            await _focusAction.Invoke();
        }
    }
}
