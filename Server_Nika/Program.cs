using Server_Nika;

internal class Program
{
    private static void Main(string[] args)
    {
        int port = 5000;
        Server server = new Server(port);
        server.Start();
    }
}