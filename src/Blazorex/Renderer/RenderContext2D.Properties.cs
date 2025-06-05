namespace Blazorex.Renderer;

internal sealed partial class RenderContext2D : IRenderContext
{
    private object _fillStyle;

    public object FillStyle
    {
        get => _fillStyle;
        set
        {
            _fillStyle = value;
            this.SetProperty("fillStyle", value);
        }
    }

    private string _strokeStyle;

    public string StrokeStyle
    {
        get => _strokeStyle;
        set
        {
            _strokeStyle = value;
            this.SetProperty("strokeStyle", value);
        }
    }

    private int _lineWidth;

    public int LineWidth
    {
        get => _lineWidth;
        set
        {
            _lineWidth = value;
            this.SetProperty("lineWidth", value);
        }
    }

    private string _font;

    public string Font
    {
        get => _font;
        set
        {
            _font = value;
            this.SetProperty("font", value);
        }
    }

    private TextAlign _textAlign;

    public TextAlign TextAlign
    {
        get => _textAlign;
        set
        {
            _textAlign = value;
            this.SetProperty("textAlign", value.Value);
        }
    }

    private TextBaseline _textBaseline;

    public TextBaseline TextBaseline
    {
        get => _textBaseline;
        set
        {
            _textBaseline = value;
            this.SetProperty("textBaseline", value.Value);
        }
    }

    private TextRendering _textRendering;

    public TextRendering TextRendering
    {
        get => _textRendering;
        set
        {
            _textRendering = value;
            this.SetProperty("textRendering", value.Value);
        }
    }

    private Filter _filter;

    public Filter Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            this.SetProperty("filter", value.Value);
        }
    }

    private float _globalAlpha;

    public float GlobalAlpha
    {
        get => _globalAlpha;
        set
        {
            _globalAlpha = value;
            this.SetProperty("globalAlpha", value);
        }
    }

    private GlobalCompositeOperation _globalCompositeOperation;

    public GlobalCompositeOperation GlobalCompositeOperation
    {
        get => _globalCompositeOperation;
        set
        {
            _globalCompositeOperation = value;
            this.SetProperty("globalCompositeOperation", value.Value);
        }
    }

    private ImageSmoothingQuality _imageSmoothingQuality;

    public ImageSmoothingQuality ImageSmoothingQuality
    {
        get => _imageSmoothingQuality;
        set
        {
            _imageSmoothingQuality = value;
            this.SetProperty("imageSmoothingQuality", value.Value);
        }
    }

    private bool _imageSmoothingEnabled;

    public bool ImageSmoothingEnabled
    {
        get => _imageSmoothingEnabled;
        set
        {
            _imageSmoothingEnabled = value;
            this.SetProperty("imageSmoothingEnabled", value);
        }
    }
}
