namespace StorageService.Exceptions;

public class StorageException<T> : Exception where T : class 
{
    public StorageException(string message) : base(message)
    {
        Result = FileResultGeneric<T>.Failure(message);
    }
    
    public FileResultGeneric<T> Result { get; private set; }
}