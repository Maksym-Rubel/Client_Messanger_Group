namespace Server_Nika
{
    public static class FileTransferService
    {
        public static void HandleFile(ChatMessage message)
        {
            if (message.FileBytes != null && !string.IsNullOrEmpty(message.FileName))
            {
                byte[] filebytes = Convert.FromBase64String(message.FileBytes);
                var path = Path.Combine("ReceivedFiles", message.FileName);
                Directory.CreateDirectory("ReceivedFiles");
                File.WriteAllBytes(path, filebytes);

                Console.WriteLine($"File saved: {path}");
            }
        }
    }
}
