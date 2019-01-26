namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Version();
        string Sender();
        string ReplyTo();
        string BinPath();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _version;
        private readonly string _sender;
        private readonly string _replyto;
        private readonly string _binpath;

        public AppProvider(
            string version,
            string sender,
            string replyto,
            string binpAth)
        {
            _version = version;
            _sender = sender;
            _replyto = replyto;
            _binpath = binpAth;
        }

        public string Version() => _version;
        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public string BinPath() => _binpath;
    }
}