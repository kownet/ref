using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using Ref.Sites.Scrapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.App.Core
{
    public class RefService
    {
        private readonly ILogger<RefService> _logger;

        private readonly Func<SiteType, ISiteToScrapp> _siteAccessor;
        private readonly IAppProvider _appProvider;

        private readonly ICitiesRepository _citiesRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly ISiteRepository _siteRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public RefService(
            ILogger<RefService> logger,
            Func<SiteType, ISiteToScrapp> siteAccessor,
            IAppProvider appProvider,
            ICitiesRepository citiesRepository,
            IOfferRepository offerRepository,
            ISiteRepository siteRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _citiesRepository = citiesRepository;
            _offerRepository = offerRepository;
            _siteRepository = siteRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Crawl()
        {
            var successTries = 0;

            var sitesDefined = _appProvider.Sites().Select(s => (SiteType)s);

            var availableSites = (await _siteRepository.GetAllAsync())
                .Where(s => sitesDefined.Contains(s.Type))
                .Where(s => s.IsActive);

            var cities = await _citiesRepository.GetAllAsync();

            var dealTypes = _appProvider.Deals().Select(s => (DealType)s);

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    if (cities.AnyAndNotNull())
                    {
                        foreach (var city in cities)
                        {
                            foreach (var site in availableSites)
                            {
                                foreach (DealType dealType in dealTypes)
                                {
                                    var old = await _offerRepository
                                        .FindByAsync(c =>
                                            c.CityId == city.Id &&
                                            c.Site == site.Type &&
                                            c.Deal == dealType);

                                    var oldest = old.ToList();

                                    var result = _siteAccessor(site.Type).Scrapp(city, dealType);

                                    if (result.WeAreBanned)
                                    {
                                        _logger.LogError(Labels.BannedMsg(site.Type.ToString()));

                                        site.IsActive = false;

                                        await _siteRepository.UpdateAsync(site);

                                        if(_appProvider.AdminNotification())
                                        {
                                            _pushOverNotification.Send(
                                                Labels.BannedMsgTitle,
                                                Labels.BannedMsg(site.Type.ToString()));
                                        }
                                    }

                                    if (result.ThereAreNoRecords)
                                    {
                                        _logger.LogError(Labels.NoRecordsMsg(site.Type.ToString()));
                                        
                                        if(_appProvider.AdminNotification())
                                        {
                                            _pushOverNotification.Send(
                                                Labels.NoRecordsMsgTitle,
                                                Labels.NoRecordsMsg(site.Type.ToString()));
                                        }
                                    }

                                    if (result.ExceptionAccured)
                                    {
                                        _logger.LogError(Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage));
                                        
                                        if(_appProvider.AdminNotification())
                                        {
                                            _pushOverNotification.Send(
                                                Labels.ExceptionMsgTitle,
                                                Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage));
                                        }
                                    }

                                    var newestFromCriteria = result.Offers.ToList();

                                    if (site.Type != SiteType.Adresowo &&
                                        site.Type != SiteType.DomiPorta &&
                                        site.Type != SiteType.Gratka &&
                                        site.Type != SiteType.Morizon)
                                    {
                                        newestFromCriteria = newestFromCriteria
                                            .DistinctBy(p => p.Header)
                                            .ToList();
                                    }

                                    var newestFrom = newestFromCriteria
                                        .Where(p => oldest.All(p2 => p2.SiteOfferId != p.SiteOfferId))
                                        .ToList();

                                    if (newestFrom.AnyAndNotNull())
                                    {
                                        _offerRepository.BulkInsert(newestFrom);
                                    }

                                    _logger.LogTrace(
                                        $"Site {site.Type.ToString()}, Deal {dealType.ToString()}, " +
                                        $"City {city.NameRaw}, " +
                                        $"collected {newestFromCriteria.Count()} records," +
                                        $" {newestFrom.Count()} new.");
                                }

                                Thread.Sleep(_appProvider.PauseTime());
                            }

                            Thread.Sleep(_appProvider.PauseTime());
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