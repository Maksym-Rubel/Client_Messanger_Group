using Server_Nika;

internal class Program
{
    private static void Main(string[] args)
    {
        Server server = new Server(port: 5000);
        server.Start();
    }
}