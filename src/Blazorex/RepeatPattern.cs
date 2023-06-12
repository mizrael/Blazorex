namespace Blazorex
{
    public record RepeatPattern : CanvasProperty<string>
    {
        private RepeatPattern(string value) : base(value) { }
        public static readonly RepeatPattern Repeat = new ("repeat");
        public static readonly RepeatPattern RepeatX = new ("repeat-x");
        public static readonly RepeatPattern RepeatY = new ("repeat-y");
        public static readonly RepeatPattern NoRepeat = new ("no-repeat");
    }
}