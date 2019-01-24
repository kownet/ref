namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Version();
        string Sender();
        string ReplyTo();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _version;
        private readonly string _sender;
        private readonly string _replyto;

        public AppProvider(
            string version,
            string sender,
            string replyto)
        {
            _version = version;
            _sender = sender;
            _replyto = replyto;
        }

        public string Version() => _version;
        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
    }
}