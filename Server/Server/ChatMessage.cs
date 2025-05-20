public class ChatMessage
{
    public string Type { get; set; } = ""; 
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public string Content { get; set; } = "";
    public byte[]? FileBytes { get; set; }
    public string? FileName { get; set; }
}
