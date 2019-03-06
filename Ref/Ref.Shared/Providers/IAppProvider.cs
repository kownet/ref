using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppProvider : IAppBaseProvider
    {
        string Bcc();
        IEnumerable<int> Sites();
        IEnumerable<int> Deals();
        int Timeout();
    }

    public class AppProvider : AppBaseProvider, IAppProvider
    {
        private readonly string _bcc;
        private readonly string _sites;
        private readonly string _deals;
        private readonly string _timeout;

        public AppProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId,
            string bcc,
            string sites,
            string deals,
            string timeout)
            : base(address, sender, replyto, pausetime, adminnotification, successTries, appId)
        {
            _bcc = bcc;
            _sites = sites;
            _deals = deals;
            _timeout = timeout;
        }

        public string Bcc() => _bcc;
        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
        public IEnumerable<int> Deals() => _deals.Split(",").Select(int.Parse).ToList();
        public int Timeout() => int.Parse(_timeout);
    }
}