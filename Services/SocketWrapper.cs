namespace FamilyChat_Server;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class SocketWrapper
{
    public class MessagedReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    public delegate void MessageReceived(Object sender, MessagedReceivedEventArgs e);
    public event MessageReceived? OnMessageReceived;

    private WebSocket _socket;

    public SocketWrapper(WebSocket socket)
    {
        _socket = socket;
    }

    public async Task Listen()
    {
        byte[] buffer = new byte[500];
        var rcvResult = await _socket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        while (!rcvResult.CloseStatus.HasValue && _socket.State == WebSocketState.Open)
        {
            rcvResult = await _socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            // Decode the message
            string msg = Encoding.UTF8.GetString(buffer);
            OnMessageReceived?.Invoke(this, new MessagedReceivedEventArgs()
            {
                Message = msg
            });
        }
    }

    public async Task Send(string message)
    {
        await _socket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }
}
