using System.Collections.Generic;
using System.Linq;

namespace Ref.Shared.Providers
{
    public interface IAppProvider
    {
        string Sender();
        string ReplyTo();
        string BinPath();
        int PauseTime();
        IEnumerable<int> Sites();
    }

    public class AppProvider : IAppProvider
    {
        private readonly string _sender;
        private readonly string _replyto;
        private readonly string _binpath;
        private readonly string _pausetime;
        private readonly string _sites;

        public AppProvider(
            string sender,
            string replyto,
            string binpAth,
            string pausetime,
            string sites)
        {
            _sender = sender;
            _replyto = replyto;
            _binpath = binpAth;
            _pausetime = pausetime;
            _sites = sites;
        }

        public string Sender() => _sender;
        public string ReplyTo() => _replyto;
        public string BinPath() => _binpath;
        public int PauseTime() => int.Parse(_pausetime);
        public IEnumerable<int> Sites() => _sites.Split(",").Select(int.Parse).ToList();
    }
}