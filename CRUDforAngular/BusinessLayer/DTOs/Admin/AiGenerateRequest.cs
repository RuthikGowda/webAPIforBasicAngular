namespace CRUDforAngular.BusinessLayer.DTOs.Admin
{
    public class AiGenerateRequest
    {
        public string SystemMessage { get; set; } = string.Empty;
        public string UserMessage { get; set; } = string.Empty;
    }
}
