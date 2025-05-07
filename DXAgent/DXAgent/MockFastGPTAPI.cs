
namespace DXAgent
{
    public class MockFastGPTAPI : IFastGPTAPI
    {
        private readonly List<ChatHistory> _chatHistories = [];
        public IReadOnlyList<ChatHistory> Histories => _chatHistories.AsReadOnly();

        public async Task SendChatRequest(string prompt, Action<string> consumer, Action? stateChange = null)
        {
            string generated = "This is a mock response to the prompt: " + prompt;

            _chatHistories.Add(new ChatHistory(ChatRole.User, prompt));

            var history = new ChatHistory(ChatRole.Assistant, "");
            _chatHistories.Add(history);

            stateChange?.Invoke();
            await Task.Yield();

            for (int i = 0; i < generated.Length; i++)
            {
                history.Content += generated[i];
                consumer(generated[i].ToString());
                await Task.Delay(50);
            }
        }
    }
}
