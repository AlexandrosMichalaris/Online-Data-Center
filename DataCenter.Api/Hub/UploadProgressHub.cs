using Microsoft.AspNetCore.SignalR;

namespace Data_Center.Configuration;

public class UploadProgressHub : Hub
{
    // Clients can call this method if needed
    public async Task Notify(string message)
    {
        await Clients.All.SendAsync("ReceiveProgress", message);
    }
}