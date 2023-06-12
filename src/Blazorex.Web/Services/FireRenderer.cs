using Microsoft.AspNetCore.Components;

namespace Blazorex.Web.Services
{
    public class FireRenderer
    {
        private readonly IRenderContext _context;

        private int renderTarget;
        private int fireWidth = 300;
        private int fireHeight = 100;
        private byte fireStartIntensity = 36;
        private byte[] fireColorData;
        private byte[] fireData;

        private readonly Color[] palette = new Color[] {
            new Color(r:7, g:7, b:7), new Color(r:31,g:7,b:7),new Color(r:47,g:15,b:7),new Color(r:71,g:15,b:7),new Color(r:87,g:23,b:7),new Color(r:103,g:31,b:7),new Color(r:119,g:31,b:7),new Color(r:143,g:39,b:7),new Color(r:159,g:47,b:7),new Color(r:175,g:63,b:7),new Color(r:191,g:71,b:7),new Color(r:199,g:71,b:7),new Color(r:223,g:79,b:7),new Color(r:223,g:87,b:7),new Color(r:223,g:87,b:7),new Color(r:215,g:95,b:7),new Color(r:215,g:95,b:7),new Color(r:215,g:103,b:15),new Color(r:207,g:111,b:15),new Color(r:207,g:119,b:15),new Color(r:207,g:127,b:15),new Color(r:207,g:135,b:23),new Color(r:199,g:135,b:23),new Color(r:199,g:143,b:23),new Color(r:199,g:151,b:31),new Color(r:191,g:159,b:31),new Color(r:191,g:159,b:31),new Color(r:191,g:167,b:39),new Color(r:191,g:167,b:39),new Color(r:191,g:175,b:47),new Color(r:183,g:175,b:47),new Color(r:183,g:183,b:47),new Color(r:183,g:183,b:55),new Color(r:207,g:207,b:111),new Color(r:223,g:223,b:159),new Color(r:239,g:239,b:199),new Color(r:255,g:255,b:255)
        };

        public FireRenderer(IRenderContext context)
        {
            _context = context;

            renderTarget = _context.CreateImageData(fireWidth, fireHeight);

            fireColorData = new byte[fireWidth * fireHeight * 4];

            fireData = new byte[fireWidth * fireHeight];
            for (int i = 0; i < fireData.Length; i++)
                fireData[i] = fireStartIntensity;
        }

        public void Update()
        {
            for (int column = 0; column < fireWidth; column++)
            {
                for (int row = 0; row < fireHeight; row++)
                {
                    int pixelIndex = column + (fireWidth * row);
                    UpdateFireIntensityPerPixel(pixelIndex);
                }
            }
        }

        public void Render()
        {
            _context.PutImageData(renderTarget, fireColorData, 10, 50);
        }

        private void UpdateFireIntensityPerPixel(int currentPixelIndex)
        {
            int belowPixelIndex = currentPixelIndex + fireWidth;
            if (belowPixelIndex >= fireWidth * fireHeight)
                return;

            byte decay = (byte)(Math.Floor(Random.Shared.NextDouble() * 3));
            byte belowPixelFireIntensity = fireData[belowPixelIndex];
            byte newFireIntensity = Math.Max((byte)0, (byte)(belowPixelFireIntensity - decay));

            int fireDataIndex = currentPixelIndex - decay;
            fireData[fireDataIndex] = newFireIntensity;

            int colorIndex = fireDataIndex * 4;
            var color = (newFireIntensity >= palette.Length) ? Color.White : palette[newFireIntensity];
            fireColorData[colorIndex] = color.r;
            fireColorData[colorIndex + 1] = color.g;
            fireColorData[colorIndex + 2] = color.b;
            fireColorData[colorIndex + 3] = 255;
        }
    }

    public record Color(byte r, byte g, byte b)
    {
        public readonly static Color White = new Color(255, 255, 255);
    }

}
