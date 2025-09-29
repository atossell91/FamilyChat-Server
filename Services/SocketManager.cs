using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace FamilyChat_Server;

public class SocketManager
{
    private Dictionary<string, SocketWrapper> sockets;
    public SocketManager()
    {
        sockets = new();
    }

    void HandleConnection(MessagePacket packet, SocketWrapper socket)
    {
        if (!sockets.ContainsKey(packet.Name))
        {
            sockets.Add(packet.Name, socket);
        }
    }

    async Task HandleChatMessage(MessagePacket packet)
    {
        Console.WriteLine($"Name {packet.Name}");
        foreach (string user in packet.Target)
        {
            if (sockets.ContainsKey(user))
            {
                Console.WriteLine($"Sending to {user}");
                await sockets[user].Send(packet);
            }
        }
    }

    void HandleMessage(object Sender, SocketWrapper.MessagedReceivedEventArgs e)
    {
        var packet = e.MessagePacket;
        Console.WriteLine($"Type: {packet.Type}, Message: {packet.Message}");
        SocketWrapper socket = (SocketWrapper)Sender;
        switch (packet.Type)
        {
            case MessageType.Connection:
                HandleConnection(packet, socket);
                break;
            case MessageType.Chat:
                HandleChatMessage(packet);
                break;
            default:
                Console.WriteLine($"Message Type not recognized: {packet.Type}");
                break;
        }
    }

    public async Task ManageSocket(WebSocket socket)
    {
        SocketWrapper wrapper = new SocketWrapper(socket);
        wrapper.OnMessageReceived += HandleMessage;
        await wrapper.Listen();
    }
}
