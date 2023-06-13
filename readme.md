# Blazorex

## Description
Blazorex is an HTML Canvas wrapper library for Blazor.

![Blazorex](https://raw.githubusercontent.com/mizrael/Blazorex/master/sample.gif "Blazorex")

It has some interesting functionalities like:
- multiple canvases
- background rendering
- image rendering
- procedural image generation (yes, the fire on the background is fully procedural!
Thanks [filipedeschamps](https://github.com/filipedeschamps) for the awesome repository showing how to render the [Doom fire](https://github.com/filipedeschamps/doom-fire-algorithm)! )

## Installation
Blazorex can be installed as Nuget package: https://www.nuget.org/packages/Blazorex/

## Usage

Simply add the `Canvas` Component to your Razor page and register to the `OnCanvasReady` to receive the `CanvasBase` instance.

Then use `OnFrameReady` to define your update/rendere logic:

```csharp
<Canvas Width="800" Height="600" 
        OnFrameReady="(t) => OnFrameReady(t)"
        OnCanvasReady="(ctx) => OnCanvasReady(ctx)" />

@code{
    CanvasBase _canvas;

    private void OnCanvasReady(CanvasBase canvas)
    {
        _canvas = canvas;
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

A sample game can be found here: [Blazeroids](https://github.com/mizrael/Blazeroids)
