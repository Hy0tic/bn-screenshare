using System.Net.WebSockets;
using System.Text;

namespace bnAPI.Janus_gateway;

public class JanusWebSocketService : IHostedService
{
    private readonly Uri _janusUri = new Uri("ws://localhost:8188/");

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ListenForEventsAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Here you'd want to close the WebSocket connection and clean up resources.
        // For now, we'll just return a completed task.
        return Task.CompletedTask;
    }

    private async Task ListenForEventsAsync()
    {
        // using var clientWebSocket = new ClientWebSocket();
        // await clientWebSocket.ConnectAsync(_janusUri, CancellationToken.None);
        //
        // var buffer = new byte[1024 * 4];
        // while (true)
        // {
        //     var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //
        //     if (result.MessageType == WebSocketMessageType.Text)
        //     {
        //         var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //         HandleJanusEvent(message);
        //     }
        //     else if (result.MessageType == WebSocketMessageType.Close)
        //     {
        //         // Handle closing of the connection if needed.
        //         break;
        //     }
        // }
    }

    private void HandleJanusEvent(string message)
    {
        // TODO: Process the message, store in a database, broadcast to other clients, etc.
        Console.WriteLine($"Received: {message}");
    }
}