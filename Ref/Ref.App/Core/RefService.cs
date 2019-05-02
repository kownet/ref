using Microsoft.Extensions.Logging;
using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using Ref.Sites.Scrapper;
using System;
using System.Collections.Generic;
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

        private readonly ICitiesReport _citiesReport;
        private readonly IOfferRepository _offerRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IEventRepository _eventRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public RefService(
            ILogger<RefService> logger,
            Func<SiteType, ISiteToScrapp> siteAccessor,
            IAppProvider appProvider,
            ICitiesReport citiesReport,
            IOfferRepository offerRepository,
            ISiteRepository siteRepository,
            IEventRepository eventRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _citiesReport = citiesReport;
            _offerRepository = offerRepository;
            _siteRepository = siteRepository;
            _eventRepository = eventRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Crawl()
        {
            var successTries = 0;

            var sitesDefined = _appProvider.Sites().Select(s => (SiteType)s);

            var availableSites = (await _siteRepository.GetAllAsync())
                .Where(s => sitesDefined.Contains(s.Type))
                .Where(s => s.IsActive);

            var cities = await _citiesReport.GetAllCitiesForActiveUsersAsync();

            var dealTypes = _appProvider.Deals().Select(s => (DealType)s);

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    if (cities.AnyAndNotNull())
                    {
                        foreach (var city in cities)
                        {
                            if (city.HasDistricts)
                            {
                                var activeDistricts = await _citiesReport.GetAllDistrictsForActiveCityAsync(city.Id);

                                if (activeDistricts.AnyAndNotNull())
                                {
                                    foreach (var district in activeDistricts)
                                    {
                                        await LoopForData(availableSites, dealTypes, city, district);
                                    }
                                }
                            }

                            await LoopForData(availableSites, dealTypes, city, null);
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

                    if (_appProvider.AppExceptionNotifications())
                    {
                        _pushOverNotification.Send(
                            $"[{_appProvider.AppId()}]{Labels.ErrorMsgTitle}",
                            $"{msgHeader} {ex.GetFullMessage()}");
                    }

                    Thread.Sleep(_appProvider.PauseTime());
                }
            }

            return 0;
        }

        private async Task LoopForData(IEnumerable<Site> availableSites, IEnumerable<DealType> dealTypes, City city, District district)
        {
            foreach (var site in availableSites)
            {
                foreach (DealType dealType in dealTypes)
                {
                    var result = _siteAccessor(site.Type).Scrapp(city, dealType, district);

                    if (result.WeAreBanned)
                    {
                        _logger.LogError(Labels.BannedMsg(site.Type.ToString()));

                        site.IsActive = false;

                        await _siteRepository.UpdateAsync(site);

                        if (_appProvider.BannedNotifications())
                        {
                            _pushOverNotification.Send(
                                Labels.BannedMsgTitle,
                                Labels.BannedMsg(site.Type.ToString()));
                        }
                    }

                    if (result.ThereAreNoRecords)
                    {
                        _logger.LogError(Labels.NoRecordsMsg(site.Type.ToString(), city.Name));

                        if (_appProvider.NoRecordsNotifications())
                        {
                            _pushOverNotification.Send(
                                Labels.NoRecordsMsgTitle,
                                Labels.NoRecordsMsg(site.Type.ToString(), city.Name));
                        }
                    }

                    if (result.ExceptionAccured)
                    {
                        _logger.LogError(Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage, city.Name));

                        if (_appProvider.ExceptionNotifications())
                        {
                            _pushOverNotification.Send(
                                Labels.ExceptionMsgTitle,
                                Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage, city.Name));
                        }
                    }

                    var newestFromCriteria = result.Offers;

                    if (site.Type != SiteType.Adresowo &&
                        site.Type != SiteType.DomiPorta &&
                        site.Type != SiteType.Gratka &&
                        site.Type != SiteType.Morizon)
                    {
                        newestFromCriteria = newestFromCriteria
                            .DistinctBy(p => p.Header);
                    }

                    if (!(district is null))
                    {
                        if (newestFromCriteria.AnyAndNotNull())
                        {
                            foreach (var item in newestFromCriteria)
                            {
                                var r = (await _offerRepository
                                    .FindByAsync(o =>
                                        o.SiteOfferId == item.SiteOfferId &&
                                        !o.DistrictId.HasValue))
                                    .FirstOrDefault();

                                if (!(r is null))
                                {
                                    await _offerRepository.SetDistrict(r.Id, district.Id);
                                }
                            }
                        }
                    }

                    var newestFrom = Enumerable.Empty<Offer>();

                    if (newestFromCriteria.AnyAndNotNull())
                    {
                        var old = await _offerRepository.
                                GetScrapped(city.Id, site.Type, dealType);

                        newestFrom = newestFromCriteria
                            .Where(p => old.All(p2 => p2 != p.SiteOfferId));
                    }

                    if (newestFrom.AnyAndNotNull())
                    {
                        _offerRepository.BulkInsert(newestFrom);
                    }

                    var districted = district is null ? $"\"District\": \"Empty\"," : $"\"District\": \"{district.NameRaw}\",";

                    var msg =
                        $"{{\"Site\": \"{site.Type.ToString()}\"," +
                        $"\"Deal\": \"{dealType.ToString()}\"," +
                        $"\"City\": \"{city.NameRaw}\"," +
                        districted +
                        $"\"Records\": {newestFromCriteria.Count()}," +
                        $"\"New\": {newestFrom.Count()} }}";

                    _logger.LogTrace(msg);

                    if (_appProvider.EventUpdate())
                    {
                        try
                        {
                            await _eventRepository.Upsert(new Event
                            {
                                Type = EventType.Success,
                                Category = (EventCategory)site.Type,
                                Message = msg
                            });
                        }
                        catch (Exception ex)
                        {
                            var msgException = $"Message: {ex.GetFullMessage()}, StackTrace: {ex.StackTrace}";

                            _logger.LogError(msgException);
                        }
                    }
                }

                Thread.Sleep(_appProvider.PauseTime());
            }

            Thread.Sleep(_appProvider.PauseTime());
        }
    }
}