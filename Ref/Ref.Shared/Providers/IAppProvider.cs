using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Address();
        string Sender();
        string ReplyTo();
        string Bcc();
        int PauseTime();
        int Timeout();
        IEnumerable<int> Sites();
        string AppId();
        bool AdminNotification();
        int SuccessTries();
        int Mode();
        IEnumerable<int> Deals();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _address;
        private readonly string _sender;
        private readonly string _replyto;
        private readonly string _bcc;
        private readonly string _pausetime;
        private readonly string _timeout;
        private readonly string _sites;
        private readonly string _appId;
        private readonly string _adminnotification;
        private readonly string _successTries;
        private readonly string _mode;
        private readonly string _deals;

        public AppProvider(
            string address,
            string sender,
            string replyto,
            string bcc,
            string pausetime,
            string timeout,
            string sites,
            string appId,
            string adminnotification,
            string successTries,
            string mode,
            string deals)
        {
            _address = address;
            _sender = sender;
            _replyto = replyto;
            _bcc = bcc;
            _pausetime = pausetime;
            _timeout = timeout;
            _sites = sites;
            _appId = appId;
            _adminnotification = adminnotification;
            _successTries = successTries;
            _mode = mode;
            _deals = deals;
        }

        public string Address() => _address;
        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public string Bcc() => _bcc;
        public int PauseTime() => int.Parse(_pausetime);
        public int Timeout() => int.Parse(_timeout);
        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
        public string AppId() => _appId;
        public bool AdminNotification() => bool.Parse(_adminnotification);
        public int SuccessTries() => int.Parse(_successTries);
        public int Mode() => int.Parse(_mode);
        public IEnumerable<int> Deals() => _deals.Split(",").Select(int.Parse).ToList();
    }
}