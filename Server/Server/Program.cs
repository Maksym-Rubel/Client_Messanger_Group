class Program
{
    static void Main(string[] args)
    {
        Server server = new Server(port: 5000);
        server.Start();
    }
}
