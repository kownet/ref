using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppProvider : IAppBaseProvider
    {
        IEnumerable<int> Sites();
        IEnumerable<int> Deals();
        int Timeout();
        int Pages();
        bool BannedNotifications();
        bool NoRecordsNotifications();
        bool ExceptionNotifications();
        bool AppExceptionNotifications();
        int CheckInterval();
        bool EventUpdate();
    }

    public class AppProvider : AppBaseProvider, IAppProvider
    {
        private readonly string _sites;
        private readonly string _deals;
        private readonly string _timeout;
        private readonly string _pages;
        private readonly string _bannedn;
        private readonly string _norecordsn;
        private readonly string _exceptionn;
        private readonly string _appexceptionn;
        private readonly string _checkinterval;
        private readonly string _eventupdate;

        public AppProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId,
            string sites,
            string deals,
            string timeout,
            string pages,
            string bannedn,
            string norecordsn,
            string exceptionn,
            string appexceptionn,
            string checkinterval,
            string eventupdate)
            : base(address, sender, replyto, pausetime, adminnotification, successTries, appId)
        {
            _sites = sites;
            _deals = deals;
            _timeout = timeout;
            _pages = pages;
            _bannedn = bannedn;
            _norecordsn = norecordsn;
            _exceptionn = exceptionn;
            _appexceptionn = appexceptionn;
            _checkinterval = checkinterval;
            _eventupdate = eventupdate;
        }

        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
        public IEnumerable<int> Deals() => _deals.Split(",").Select(int.Parse).ToList();
        public int Timeout() => int.Parse(_timeout);
        public int Pages() => int.Parse(_pages);

        public bool BannedNotifications() => bool.Parse(_bannedn);
        public bool NoRecordsNotifications() => bool.Parse(_norecordsn);
        public bool ExceptionNotifications() => bool.Parse(_exceptionn);
        public bool AppExceptionNotifications() => bool.Parse(_appexceptionn);

        public int CheckInterval() => int.Parse(_checkinterval);
        public bool EventUpdate() => bool.Parse(_eventupdate);
    }
}