namespace Blazorex.Web.Services
{
    public record Color(byte R, byte G, byte B)
    {
        public readonly static Color White = new(255, 255, 255);
    }

}
