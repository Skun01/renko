namespace project_z_backend.Share;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFalure => !IsSuccess;
    public Error? Error { get; }
    public Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException("Success MUST NOT have error");
        if (!isSuccess && error == null)
            throw new InvalidOperationException("Failure MUST HAVE Error");

        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T? value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result 
{
    public T? Value { get; }
    protected internal Result(T? value, bool isSuccess, Error? error) 
        : base(isSuccess, error)
    {
        Value = value;
    }
}
