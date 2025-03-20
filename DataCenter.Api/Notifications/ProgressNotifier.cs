using Data_Center.Configuration;
using Microsoft.AspNetCore.SignalR;
using StorageService.Service.Interface;

namespace Data_Center.Notifications;

public class ProgressNotifier : IProgressNotifier
{
    private readonly IHubContext<UploadProgressHub> _hubContext;

    public ProgressNotifier(IHubContext<UploadProgressHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task ReportProgressAsync(string connectionId, int percentage)
    {
        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveProgress", percentage);
    }
}