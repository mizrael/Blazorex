namespace Blazorex
{
    public record TextAlign : CanvasProperty<string>
    {
        private TextAlign(string value) : base(value) { }
        public static readonly TextAlign Left = new ("left");
        public static readonly TextAlign Right = new ("right");
        public static readonly TextAlign Center = new ("center");
        public static readonly TextAlign Start = new ("start");
        public static readonly TextAlign End = new ("end");
    }
}