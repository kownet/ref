using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Sender();
        string ReplyTo();
        string Bcc();
        int PauseTime();
        IEnumerable<int> Sites();
        string AppId();
        bool AdminNotification();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _sender;
        private readonly string _replyto;
        private readonly string _bcc;
        private readonly string _pausetime;
        private readonly string _sites;
        private readonly string _appId;
        private readonly string _adminnotification;

        public AppProvider(
            string sender,
            string replyto,
            string bcc,
            string pausetime,
            string sites,
            string appId,
            string adminnotification)
        {
            _sender = sender;
            _replyto = replyto;
            _bcc = bcc;
            _pausetime = pausetime;
            _sites = sites;
            _appId = appId;
            _adminnotification = adminnotification;
        }

        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public string Bcc() => _bcc;
        public int PauseTime() => int.Parse(_pausetime);
        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
        public string AppId() => _appId;
        public bool AdminNotification() => bool.Parse(_adminnotification);
    }
}