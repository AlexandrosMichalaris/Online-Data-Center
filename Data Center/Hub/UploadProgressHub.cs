using Microsoft.AspNetCore.SignalR;

namespace Data_Center.Configuration;

public class UploadProgressHub : Hub
{
    // public override async Task OnConnectedAsync()
    // {
    //     var connectionId = Context.ConnectionId;
    //     
    //     // Log the connection ID on the server
    //     Console.WriteLine($"Client connected with ConnectionId: {connectionId}");
    //
    //     await base.OnConnectedAsync();
    // }
    
    // Clients can call this method if needed
    public async Task Notify(string message)
    {
        await Clients.All.SendAsync("ReceiveProgress", message);
    }
    
    // // Optionally, handle disconnection
    // public override async Task OnDisconnectedAsync(Exception exception)
    // {
    //     var connectionId = Context.ConnectionId;
    //     Console.WriteLine($"Client disconnected with ConnectionId: {connectionId}");
    //     await base.OnDisconnectedAsync(exception);
    // }
}