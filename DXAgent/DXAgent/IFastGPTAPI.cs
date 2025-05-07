namespace DXAgent
{
    public interface IFastGPTAPI
    {
        public Task SendChatRequest(string prompt, Action<string> consumer, Action? stateChange = null);
        public IReadOnlyList<ChatHistory> Histories { get; }
    }
}
