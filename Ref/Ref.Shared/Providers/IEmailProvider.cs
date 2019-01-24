namespace Ref.Shared.Providers
{
    public interface IEmailProvider
    {
        string Host();
        string ApiKey();
        string Recipients();
    }

    public class EmailProvider : IEmailProvider
    {
        private readonly string _host;
        private readonly string _apikey;
        private readonly string _recipients;

        public EmailProvider(
            string host,
            string apikey,
            string recipients)
        {
            _host = host;
            _apikey = apikey;
            _recipients = recipients;
        }

        public string Host() => _host;
        public string ApiKey() => _apikey;
        public string Recipients() => _recipients;
    }
}