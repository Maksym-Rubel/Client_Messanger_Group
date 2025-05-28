namespace Server_Nika
{
    public class ChatMessage
    {
        public string Type { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Email { get; set; } = "";
        public int chatid { get; set; } = 0;

        public string Content { get; set; } = "";
        public string DateTime { get; set; } = "";

        public string FileBytes { get; set; } = "";


        public string? FileName { get; set; }
    }
}
