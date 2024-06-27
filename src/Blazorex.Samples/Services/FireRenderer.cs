namespace Blazorex.Samples.Services;

public class FireRenderer
{
    private readonly IRenderContext _context;

    private readonly int _renderTarget;
    private readonly int _width;
    private readonly int _height;
    private readonly byte fireStartIntensity = 36;
    private readonly byte[] fireColorData;
    private readonly byte[] fireData;

    private readonly Color[] palette = new Color[] {
        new Color(R:7, G:7, B:7), new Color(R:31,G:7,B:7),new Color(R:47,G:15,B:7),new Color(R:71,G:15,B:7),new Color(R:87,G:23,B:7),new Color(R:103,G:31,B:7),new Color(R:119,G:31,B:7),new Color(R:143,G:39,B:7),new Color(R:159,G:47,B:7),new Color(R:175,G:63,B:7),new Color(R:191,G:71,B:7),new Color(R:199,G:71,B:7),new Color(R:223,G:79,B:7),new Color(R:223,G:87,B:7),new Color(R:223,G:87,B:7),new Color(R:215,G:95,B:7),new Color(R:215,G:95,B:7),new Color(R:215,G:103,B:15),new Color(R:207,G:111,B:15),new Color(R:207,G:119,B:15),new Color(R:207,G:127,B:15),new Color(R:207,G:135,B:23),new Color(R:199,G:135,B:23),new Color(R:199,G:143,B:23),new Color(R:199,G:151,B:31),new Color(R:191,G:159,B:31),new Color(R:191,G:159,B:31),new Color(R:191,G:167,B:39),new Color(R:191,G:167,B:39),new Color(R:191,G:175,B:47),new Color(R:183,G:175,B:47),new Color(R:183,G:183,B:47),new Color(R:183,G:183,B:55),new Color(R:207,G:207,B:111),new Color(R:223,G:223,B:159),new Color(R:239,G:239,B:199),new Color(R:255,G:255,B:255)
    };

    public FireRenderer(IRenderContext context, int width, int height)
    {
        _context = context;

        _width = width;
        _height = height;

        _renderTarget = _context.CreateImageData(_width, _height);

        fireColorData = new byte[_width * _height * 4];

        fireData = new byte[_width * _height];
        for (int i = 0; i < fireData.Length; i++)
            fireData[i] = fireStartIntensity;
    }

    public void Update()
    {
        for (int column = 0; column < _width; column++)
        {
            for (int row = 0; row < _height; row++)
            {
                int pixelIndex = column + (_width * row);
                UpdateFireIntensityPerPixel(pixelIndex);
            }
        }
    }

    public void Render()
    {
        _context.PutImageData(_renderTarget, fireColorData, 0, 0);
    }

    private void UpdateFireIntensityPerPixel(int currentPixelIndex)
    {
        int belowPixelIndex = currentPixelIndex + _width;
        if (belowPixelIndex >= _width * _height)
            return;

        byte decay = (byte)(Math.Floor(Random.Shared.NextDouble() * 3));
        byte belowPixelFireIntensity = fireData[belowPixelIndex];
        byte newFireIntensity = Math.Max((byte)0, (byte)(belowPixelFireIntensity - decay));

        int fireDataIndex = currentPixelIndex - decay;
        if (fireDataIndex < 0 || fireDataIndex > fireData.Length)
            return;
        fireData[fireDataIndex] = newFireIntensity;

        int colorIndex = fireDataIndex * 4;
        var color = (newFireIntensity >= palette.Length) ? Color.White : palette[newFireIntensity];
        fireColorData[colorIndex] = color.R;
        fireColorData[colorIndex + 1] = color.G;
        fireColorData[colorIndex + 2] = color.B;
        fireColorData[colorIndex + 3] = 255;
    }
}
