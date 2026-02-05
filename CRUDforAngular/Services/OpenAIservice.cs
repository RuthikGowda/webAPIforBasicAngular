using CRUDforAngular.BusinessLayer.DTOs.Admin;
using CRUDforAngular.Migrations;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CRUDforAngular.Services
{
    public class OpenAIservice : IOpenAIservice
    {
        private readonly IConfiguration _configuration;
 

        private readonly string? apiKey;
        private readonly string _deepSeekAIapiKey;
        private readonly string deepSeekEndPoint;

        

        public OpenAIservice(IConfiguration configuration)
        {
            _configuration = configuration;   
            apiKey = _configuration["OpenAISettings:ApiKey"]; 
            _deepSeekAIapiKey = _configuration["OpenAISettings:deepSeekAIApiKey"];
            deepSeekEndPoint = _configuration["OpenAISettings:DeekSeekEndPoint"];
           


        }
 
        // Pseudocode:
        // 1. Validate apiKey; throw if missing.
        // 2. Instantiate OpenAIClient with apiKey.
        // 3. Build chat messages list with user prompt.
        // 4. Call CreateChatCompletion (synchronously via GetAwaiter().GetResult()).
        // 5. Extract first choice content; return empty string if null.
        // 6. Handle exceptions and return a safe error message (or rethrow based on needs).
        public async Task<string> GetOpenAIResponse(string prompt)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI API key is not configured.");

            if (string.IsNullOrWhiteSpace(prompt))
                return string.Empty;

            try
            {
                var client = new OpenAIClient(apiKey);
                var messages = new List<ChatMessage>();
                
                messages.Add(SystemChatMessage.CreateAssistantMessage("Get the response"));
                messages.Add(UserChatMessage.CreateUserMessage(prompt));
                var response = await client.GetChatClient("gpt-4o-mini").CompleteChatAsync(messages);

 
                // NOTE: Placeholder for actual chat completion call; adjust per SDK version.
                // Example (adjust model/method names to match installed SDK):
                // var chatResponse = await client.Chat.GetChatCompletionsAsync(
                //     model: "gpt-4o-mini",
                //     messages: messages);
                //
                // var content = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
                // return content ?? string.Empty;

                return string.Empty; // Temporary until completion call is implemented.
            }
            catch (Exception ex)
            {
                return $"OpenAI error: {ex.Message}";
            }
        }

        public async Task<string> GetResponseAsync(AiGenerateRequest aiGenerateRequest)
        {
            {
                //                "systemMessage": "You are a professional marketing copywriter. Generate exactly ONE short unique, best, promotional sentence . Do not use bullet points, numbers, line breaks, or multiple options. Do not include explanations, labels, or extra text. Return only the final sentence.",
                //  "userMessage": "pizza hut 50% cashback"
                //}
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("api-key", _deepSeekAIapiKey);
                    var requestBody = new
                    {
                        model = "gpt-4o",
                        messages = new[]
                        {
                            new { role = "system", content = aiGenerateRequest.SystemMessage },
                            new { role = "user", content = aiGenerateRequest.UserMessage }
                        }
                    };

                    var requestJson = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(deepSeekEndPoint, requestJson);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        var assistentResponse = responseObject.choices[0].message.content.ToString();
                        return assistentResponse;
                    }
                    else
                    {
                        return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
            }
        }
            
    }
}
