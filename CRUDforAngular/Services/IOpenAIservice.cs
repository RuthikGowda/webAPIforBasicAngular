


using CRUDforAngular.BusinessLayer.DTOs.Admin;

namespace CRUDforAngular.Services
{
    public interface IOpenAIservice
    {
        public   Task<string> GetOpenAIResponse(string prompt);
        public Task<string> GetResponseAsync(AiGenerateRequest prompt);
    }
}
