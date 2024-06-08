namespace NetPad.Utilities;

/// <summary>
/// A generic wrapper class that wraps a value of type <see cref="T"/>.
/// </summary>
public class Accessor<T>(T value)
{
    public T Value { get; private set; } = value;

    public Accessor<T> Update(T newValue)
    {
        Value = newValue;
        return this;
    }
}
