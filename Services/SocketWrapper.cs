namespace FamilyChat_Server;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class SocketWrapper
{
    public class MessagedReceivedEventArgs : EventArgs
    {
        public MessagePacket MessagePacket { get; set; }
    }
    public delegate void MessageReceived(Object sender, MessagedReceivedEventArgs e);
    public event MessageReceived? OnMessageReceived;

    private WebSocket _socket;

    public SocketWrapper(WebSocket socket)
    {
        _socket = socket;
    }

    public async Task HandleMessage()
    {
        byte[] buffer = new byte[500];
        var rcvResult = await _socket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        if (rcvResult.CloseStatus.HasValue) return;

        // Decode the message
        string msg = Encoding.UTF8.GetString(buffer, 0, rcvResult.Count);
        var opts = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var resp = JsonSerializer.Deserialize<MessagePacket>(msg, opts);
        OnMessageReceived?.Invoke(this, new MessagedReceivedEventArgs()
        {
            MessagePacket = resp
        });
    }

    public async Task Listen()
    {
        do {
            await HandleMessage();
        } while (_socket.State == WebSocketState.Open);
    }

    public async Task Send(MessagePacket packet)
    {
        string message = JsonSerializer.Serialize<MessagePacket>(packet);
        await _socket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }
}
