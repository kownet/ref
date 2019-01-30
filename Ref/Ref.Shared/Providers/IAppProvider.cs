namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Version();
        string Sender();
        string ReplyTo();
        string BinPath();
        int PauseTime();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _version;
        private readonly string _sender;
        private readonly string _replyto;
        private readonly string _binpath;
        private readonly string _pausetime;

        public AppProvider(
            string version,
            string sender,
            string replyto,
            string binpAth,
            string pausetime)
        {
            _version = version;
            _sender = sender;
            _replyto = replyto;
            _binpath = binpAth;
            _pausetime = pausetime;
        }

        public string Version() => _version;
        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public string BinPath() => _binpath;
        public int PauseTime() => int.Parse(_pausetime);
    }
}