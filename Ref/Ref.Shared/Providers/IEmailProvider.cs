namespace Ref.Shared.Providers
{
    public interface IEmailProvider
    {
        string Host();
        string ApiKey();
    }

    public class EmailProvider : IEmailProvider
    {
        private readonly string _host;
        private readonly string _apikey;

        public EmailProvider(
            string host,
            string apikey)
        {
            _host = host;
            _apikey = apikey;
        }

        public string Host() => _host;
        public string ApiKey() => _apikey;
    }
}