# Blazorex

## Description
Blazorex is an HTML Canvas wrapper library for Blazor, written with .NET 6.


## Usage

Simply add the `Canvas` Component to your Razor page and register to the `OnCanvasReady` to receive the `IRenderContext` instance.

Then use `OnFrameReady` to define your update/rendere logic:

```csharp
<Canvas Width="800" Height="600" 
        OnFrameReady="(t) => OnFrameReady(t)"
        OnCanvasReady="(ctx) => OnCanvasReady(ctx)" />

@code{
    IRenderContext _context;

    private void OnCanvasReady(IRenderContext context)
    {
        _context = context;
    }

    private void OnFrameReady(float timeStamp)
    {
        // your render logic goes here
    }
}

```