using System.Net.Sockets;
using System.Net.WebSockets;

namespace FamilyChat_Server;

public class SocketManager
{
    private Dictionary<string, SocketWrapper> sockets;
    public SocketManager()
    {
        sockets = new();
    }

    void HandleMessage(object Sender, SocketWrapper.MessagedReceivedEventArgs e)
    {
        Console.WriteLine(e.Message);
    }

    public async Task ManageSocket(WebSocket socket)
    {
        SocketWrapper wrapper = new SocketWrapper(socket);
        wrapper.OnMessageReceived += HandleMessage;
        await wrapper.Listen();
    }
}
