namespace CRUDforAngular.BusinessLayer.Models
{
    public class smtpOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string username { get; set; }
        public string appPassword { get; set; }
    }
}
