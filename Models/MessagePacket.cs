namespace FamilyChat_Server;

public enum MessageType
{
    None = 0, // 0
    Chat = 1 << 0, // 1
    Connection = 1 << 1 // 2
}

public class MessagePacket
{
    public string? Name { get; set; }
    public string? Message { get; set; }
    public string? Target { get; set; }
    public MessageType Type { get; set; } = MessageType.None;
}
