public class PinnableReference<T>
{
    private T _value;

    public T Value
    {
        get => _value;
        set => _value = value;
    }

    public PinnableReference(T value)
    {
        this._value = value;
    }

    public ref T GetPinnableReference()
    {
        return ref _value;
    }
}