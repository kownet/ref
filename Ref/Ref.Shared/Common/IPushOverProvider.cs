namespace Ref.Shared.Common
{
    public interface IPushOverProvider
    {
        string Token();
        string Recipients();
        string Endpoint();
    }

    public class PushOverProvider : IPushOverProvider
    {
        private readonly string _token;
        private readonly string _recipients;
        private readonly string _endpoint;

        public PushOverProvider(
            string token,
            string recipients,
            string endpoint)
        {
            _token = token;
            _recipients = recipients;
            _endpoint = endpoint;
        }

        public string Token() => _token;
        public string Recipients() => _recipients;
        public string Endpoint() => _endpoint;
    }
}