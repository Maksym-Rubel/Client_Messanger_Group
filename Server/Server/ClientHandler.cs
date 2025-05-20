using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class ClientHandler
{
    private TcpClient client;
    private Server server;
    private NetworkStream stream;
    public User? User { get; private set; }

    public ClientHandler(TcpClient client, Server server)
    {
        this.client = client;
        this.server = server;
        this.stream = client.GetStream();
    }

    public void HandleClient()
    {
        try
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                string? raw;
                while ((raw = reader.ReadLine()) != null)
                {
                    ChatMessage message = JsonSerializer.Deserialize<ChatMessage>(raw)!;
                    HandleMessage(message);
                }
            }
        }
        catch
        {
            Console.WriteLine($"Client disconnected: {User?.Username}");
            server.RemoveClient(this);
        }
    }

    public void SendMessage(ChatMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(json + "\n");
        stream.Write(buffer, 0, buffer.Length);
    }

    private void HandleMessage(ChatMessage message)
    {
        switch (message.Type)
        {
            case "Login":
                User = new User { Username = message.From };
                if (!Server.AdminService.IsApproved(User.Username))
                {
                    SendMessage(new ChatMessage { Type = "System", Content = "Awaiting administrator confirmation" });
                    return;
                }
                Server.UserManager.AddUser(User);
                server.BroadcastMessage(new ChatMessage { Type = "System", Content = $"{User.Username} joined the chat" });
                break;

            case "Message":
                server.BroadcastMessage(message, message.From);
                break;

            case "Private":
                var target = server.GetClients().FirstOrDefault(c => c.User?.Username == message.To);
                target?.SendMessage(message);
                break;

            case "File":
                FileTransferService.HandleFile(message);
                break;
        }
    }
}
