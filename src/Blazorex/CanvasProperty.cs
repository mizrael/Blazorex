namespace Blazorex;

public abstract record CanvasProperty<T>
{
    public T Value { get; }
    protected CanvasProperty(T value)
    {
        this.Value = value;
    }
}