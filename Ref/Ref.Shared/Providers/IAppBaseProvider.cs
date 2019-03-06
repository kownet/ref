namespace Ref.Shared.Providers
{
    public interface IAppBaseProvider
    {
        string Address();
        string Sender();
        string ReplyTo();
        int PauseTime();
        bool AdminNotification();
        int SuccessTries();
        string AppId();
    }

    public abstract class AppBaseProvider : IAppBaseProvider
    {
        protected readonly string _address;
        protected readonly string _sender;
        protected readonly string _replyto;
        protected readonly string _pausetime;
        protected readonly string _adminnotification;
        protected readonly string _successTries;
        protected readonly string _appId;

        public AppBaseProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId)
        {
            _address = address;
            _sender = sender;
            _replyto = replyto;
            _pausetime = pausetime;
            _adminnotification = adminnotification;
            _successTries = successTries;
            _appId = appId;
        }

        public string Address() => _address;
        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public int PauseTime() => int.Parse(_pausetime);
        public bool AdminNotification() => bool.Parse(_adminnotification);
        public int SuccessTries() => int.Parse(_successTries);
        public string AppId() => _appId;
    }
}