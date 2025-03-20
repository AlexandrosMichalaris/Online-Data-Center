namespace StorageService.Service.Interface;

/// <summary>
/// This is used to invert the dependency
/// </summary>
public interface IProgressNotifier
{
    Task ReportProgressAsync(string connectionId, int percentage);
}