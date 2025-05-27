
using System.Net;
using System.Net.Sockets;


namespace Server_Nika
{
    public class Server
    {
        private TcpListener listener;
        private readonly int port;
        private List<ClientHandler> clients = new();
        public static UserManager UserManager = new();
        public static AdminService AdminService = new();

        public Server(int port)
        {
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"Server is running on port {port}");

            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ClientHandler handler = new ClientHandler(tcpClient, this);
                clients.Add(handler);
                new Thread(handler.HandleClient).Start();
            }
        }

        public void BroadcastMessage(ChatMessage msg, string? excludeUser = null)
        {
            foreach (var client in clients)
            {
                if (client.User?.Username != excludeUser)
                {
                    client.SendMessage(msg);
                }
            }
        }

        public void RemoveClient(ClientHandler handler)
        {
            clients.Remove(handler);
        }

        public List<ClientHandler> GetClients() => clients;
    }

}