using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DXAgent
{
    public class FastGPTAPI : IFastGPTAPI
    {
        private readonly List<ChatHistory> _histories = [];
        public IReadOnlyList<ChatHistory> Histories => _histories.AsReadOnly();
        private readonly HttpClient _httpClient = new();

        public FastGPTAPI(Uri baseAddress, string apikey)
        {
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apikey}");
            this._httpClient.BaseAddress = baseAddress;
        }

        public async Task SendChatRequest(string prompt, Action<string> consumer, Action? stateChange = null)
        {
            this._histories.Add(new(ChatRole.User, prompt));
            var targetHistory = new ChatHistory(ChatRole.Assistant, "思考中...");
            this._histories.Add(targetHistory);
            stateChange?.Invoke();
            await Task.Yield();

            var request = BuildRequest();
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            StringBuilder builder = new();

            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line == null)
                {
                    continue;
                }

                if (line.StartsWith("data: "))
                {
                    var json = line["data: ".Length..].Trim();
                    if (json == "[DONE]")
                    {
                        break;
                    }

                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        var deltaToken = doc.RootElement
                                    .GetProperty("choices")[0]
                                    .GetProperty("delta");
                        string? token = "";
                        if (deltaToken.TryGetProperty("reasoning_content", out var jsonElement))
                        {
                            token = jsonElement.GetString();
                        }
                        else if (deltaToken.TryGetProperty("content", out var jsonElement1))
                        {
                            token = jsonElement1.GetString();
                        }

                        builder.Append(token ?? "");
                        targetHistory.Content = builder.ToString();
                        consumer.Invoke(token ?? "");
                        
                        await Task.Yield();
                    }
                    catch
                    {
                        // ignore wrong json
                    }
                }
            }
        }

        private HttpRequestMessage BuildRequest()
        {
            JsonSerializerOptions jsonOptions = new()
            {
                Converters =
                {
                    new JsonStringEnumConverter(new LowerCaseNamingPolicy())
                },
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(new
            {
                // chatId = undefined
                stream = true,
                detail = false,
                messages = this._histories[..^1]
            }, jsonOptions);

            var content = new StringContent(json, encoding: Encoding.UTF8, mediaType: "application/json");
            return new HttpRequestMessage(HttpMethod.Post, "/api/v1/chat/completions")
            {
                Content = content
            };
        }
    }
}

file class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}