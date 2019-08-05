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

        private readonly IUserSubscriptionReport _userSubscriptionReport;

        private readonly IOfferRepository _offerRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IEventRepository _eventRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public RefService(
            ILogger<RefService> logger,
            Func<SiteType, ISiteToScrapp> siteAccessor,
            IAppProvider appProvider,
            IUserSubscriptionReport userSubscriptionReport,
            IOfferRepository offerRepository,
            ISiteRepository siteRepository,
            IEventRepository eventRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _userSubscriptionReport = userSubscriptionReport;
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

            var activeFilters = (await _userSubscriptionReport.GetAllActiveFiltersAsync())
                .Where(u => !u.DemoPassed(24));

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    if(activeFilters.AnyAndNotNull())
                    {
                        foreach (var filter in activeFilters)
                        {
                            await LookForData(availableSites, filter);
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

        private async Task LookForData(IEnumerable<Site> availableSites, UserSubscriptionFilter userSubscriptionFilter)
        {
            foreach (var site in availableSites)
            {
                var result = _siteAccessor(site.Type).Scrapp(userSubscriptionFilter);

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
                    _logger.LogError(Labels.NoRecordsMsg(site.Type.ToString(), userSubscriptionFilter.City));

                    if (_appProvider.NoRecordsNotifications())
                    {
                        _pushOverNotification.Send(
                            Labels.NoRecordsMsgTitle,
                            Labels.NoRecordsMsg(site.Type.ToString(), userSubscriptionFilter.City));
                    }
                }

                if (result.ExceptionAccured)
                {
                    _logger.LogError(Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage, userSubscriptionFilter.City));

                    if (_appProvider.ExceptionNotifications())
                    {
                        _pushOverNotification.Send(
                            Labels.ExceptionMsgTitle,
                            Labels.ExceptionMsg(site.Type.ToString(), result.ExceptionMessage, userSubscriptionFilter.City));
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

                if (!(userSubscriptionFilter.DistrictId is null))
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
                                await _offerRepository.SetDistrict(r.Id, userSubscriptionFilter.DistrictId.Value);
                            }
                        }
                    }
                }

                var newestFrom = Enumerable.Empty<Offer>();

                if (newestFromCriteria.AnyAndNotNull())
                {
                    var old = await _offerRepository.
                            GetScrapped(
                        userSubscriptionFilter.CityId,
                        site.Type,
                        userSubscriptionFilter.Deal,
                        userSubscriptionFilter.Property);

                    newestFrom = newestFromCriteria
                        .Where(p => old.All(p2 => p2 != p.SiteOfferId));
                }

                if (newestFrom.AnyAndNotNull())
                {
                    _offerRepository.BulkInsert(newestFrom);
                }

                var districted = userSubscriptionFilter.DistrictId is null ? $"\"District\": \"Empty\"," : $"\"District\": \"{userSubscriptionFilter.District}\",";

                var msg =
                    $"{{\"Site\": \"{site.Type.ToString()}\"," +
                    $"\"Deal\": \"{userSubscriptionFilter.Deal.ToString()}\"," +
                    $"\"City\": \"{userSubscriptionFilter.City}\"," +
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

                Thread.Sleep(_appProvider.PauseTime());
            }

            Thread.Sleep(_appProvider.PauseTime());
        }
    }
}