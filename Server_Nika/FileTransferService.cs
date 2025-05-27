namespace Server_Nika
{
    public static class FileTransferService
    {
        public static void HandleFile(ChatMessage message)
        {
            if (message.FileBytes != null && !string.IsNullOrEmpty(message.FileName))
            {
                var path = Path.Combine("ReceivedFiles", message.FileName);
                Directory.CreateDirectory("ReceivedFiles");
                File.WriteAllBytes(path, message.FileBytes);
                Console.WriteLine($"File saved: {path}");
            }
        }
    }
}
