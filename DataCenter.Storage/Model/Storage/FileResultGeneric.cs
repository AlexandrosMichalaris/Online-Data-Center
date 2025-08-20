public class FileResultGeneric<T> where T : class
{
    // Core Properties
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public T? Data { get; }
    public int? StatusCode { get; }

    // Private Constructor
    private FileResultGeneric(bool isSuccess, string? errorMessage, T? data, int? statusCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Data = data;
        StatusCode = statusCode;
    }

    // Static Factory Methods

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static FileResultGeneric<T> Success(T data)
    {
        return new FileResultGeneric<T>(true, null, data);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static FileResultGeneric<T> Failure(string? errorMessage, int? statusCode = null)
    {
        return new FileResultGeneric<T>(false, errorMessage, null, statusCode);
    }

    // ToString Override for Better Debugging
    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {typeof(T).Name} data included."
            : $"Failure: {ErrorMessage}";
    }
}