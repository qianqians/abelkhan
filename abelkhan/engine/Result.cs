namespace engine;

public readonly record struct Result<T, E>
{
    public bool IsOk { get; }
    public bool IsErr => !IsOk;

    public T Value { get; }
    public E Error { get; }

    private Result(T value)
    {
        IsOk = true;
        Value = value;
        Error = default!;
    }

    private Result(E error)
    {
        IsOk = false;
        Value = default!;
        Error = error;
    }

    public static Result<T, E> Ok(T value) => new(value);
    public static Result<T, E> Err(E error) => new(error);
}