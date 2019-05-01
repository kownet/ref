using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppScrapperProvider : IAppBaseProvider
    {
        IEnumerable<int> Sites();
        int Timeout();
        int ChunkSize();
        int ScrappPause();
        bool EventUpdate();
    }

    public class AppScrapperProvider : AppBaseProvider, IAppScrapperProvider
    {
        private readonly string _sites;
        private readonly string _timeout;
        private readonly string _chunkSize;
        private readonly string _scrappPause;
        private readonly string _eventupdate;

        public AppScrapperProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId,
            string sites,
            string timeout,
            string chunkSize,
            string scrappPause,
            string eventupdate)
            : base(address, sender, replyto, pausetime, adminnotification, successTries, appId)
        {
            _sites = sites;
            _timeout = timeout;
            _chunkSize = chunkSize;
            _scrappPause = scrappPause;
            _eventupdate = eventupdate;
        }

        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
        public int Timeout() => int.Parse(_timeout);
        public int ChunkSize() => int.Parse(_chunkSize);
        public int ScrappPause() => int.Parse(_scrappPause);
        public bool EventUpdate() => bool.Parse(_eventupdate);
    }
}