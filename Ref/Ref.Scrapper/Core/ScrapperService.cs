using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Scrapper.Core
{
    public class ScrapperService
    {
        private readonly ILogger<ScrapperService> _logger;

        private readonly IAppScrapperProvider _appProvider;

        private readonly IOfferRepository _offerRepository;
        private readonly ISiteRepository _siteRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public ScrapperService(
            ILogger<ScrapperService> logger,
            IAppScrapperProvider appProvider,
            IOfferRepository offerRepository,
            ISiteRepository siteRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _offerRepository = offerRepository;
            _siteRepository = siteRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Scrapp()
        {
            var successTries = 0;

            var sitesDefined = _appProvider.Sites().Select(s => (SiteType)s);

            var availableSites = (await _siteRepository.GetAllAsync())
                .Where(s => sitesDefined.Contains(s.Type))
                .Where(s => s.IsActive);

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    foreach (var site in availableSites)
                    {
                        var toScrapp = await _offerRepository
                            .FindByAsync(o => !o.IsScrapped && o.Site == site.Type);

                        if(toScrapp.AnyAndNotNull())
                        {
                            _logger.LogTrace($"Found {toScrapp.Count()} for site {site.Type.ToString()}");

                            var chunked = toScrapp.Chunk(_appProvider.ChunkSize());

                            foreach (var chunk in chunked)
                            {
                                foreach (var offer in chunk)
                                {
                                    Thread.Sleep(_appProvider.ScrappPause());
                                }

                                Thread.Sleep(_appProvider.PauseTime());
                            }
                        }
                    }

                    successTries = _appProvider.SuccessTries();
                }
                catch (Exception ex)
                {
                    successTries++;

                    var msgHeader = $"[no. {successTries}]";

                    var msgException = $"{msgHeader} Message: {ex.GetFullMessage()}, StackTrace: {ex.StackTrace}";

                    _logger.LogError(msgException);

                    _pushOverNotification.Send(
                        $"[{_appProvider.AppId()}]{Labels.ErrorMsgTitle}",
                        $"{msgHeader} {ex.GetFullMessage()}");

                    Thread.Sleep(_appProvider.PauseTime());
                }
            }

            return 0;
        }
    }
}