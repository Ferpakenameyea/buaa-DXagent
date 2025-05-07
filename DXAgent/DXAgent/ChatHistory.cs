using System.Text.Json.Serialization;

namespace DXAgent
{
    public class ChatHistory(ChatRole chatRole, string? content)
    {
        [JsonPropertyName("role")]
        public ChatRole ChatRole { get; set; } = chatRole;

        [JsonPropertyName("content")]
        public string? Content { get; set; } = content;
    }

    public enum ChatRole
    {
        Assistant, User
    }
}
