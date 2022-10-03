# Blazorex

## Description
Blazorex is an HTML Canvas wrapper library for Blazor, written with .NET 6.

![Blazorex](sample.gif "Blazorex")

## Installation
Blazorex can be installed as Nuget package: https://www.nuget.org/packages/Blazorex/

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

You might also need to update your `index.html` to include the library's CSS:
```html
<head>
    <!-- other tags... -->
    <link href="_content/Blazorex/blazorex.css" rel="stylesheet" />
</head>
```

For a complete sample, check the [./src/Blazorex.Web](./src/Blazorex.Web) folder.