using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Coordinator.Core
{
    public class CoordinatorService
    {
        private readonly ILogger<CoordinatorService> _logger;

        private readonly IAppProvider _appProvider;

        private readonly IFilterRepository _filterRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IOfferFilterRepository _offerFilterRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public CoordinatorService(
            ILogger<CoordinatorService> logger,
            IAppProvider appProvider,
            IFilterRepository filterRepository,
            IOfferRepository offerRepository,
            IOfferFilterRepository offerFilterRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _filterRepository = filterRepository;
            _offerRepository = offerRepository;
            _offerFilterRepository = offerFilterRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Manage()
        {
            var successTries = 0;

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    var oldFiltersToCheck = await _filterRepository
                        .FindByAsync(f => f.LastCheckedAt.HasValue);

                    var oldGrouped = oldFiltersToCheck.GroupBy(f => f.Notification);

                    foreach (var group in oldGrouped)
                    {
                        var oldFiltersToCheckGroup = group
                            .Where(f => 
                                f.LastCheckedAt.Value <= DateTime.Now.AddHours(-ResolveTimeInterval(group.Key)));

                        if(oldFiltersToCheckGroup.AnyAndNotNull())
                        {
                            await MatchOffers(oldFiltersToCheckGroup);
                        }
                    }

                    var newFiltersToCheck = await _filterRepository
                        .FindByAsync(f =>
                            f.Notification != NotificationType.Undefinded &&
                            !f.LastCheckedAt.HasValue);

                    if (newFiltersToCheck.AnyAndNotNull())
                    {
                        await MatchOffers(newFiltersToCheck);
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

                    Thread.Sleep(5 * 1000);
                }
            }

            return 0;
        }

        private int ResolveTimeInterval(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Immediately:
                    return 0;
                case NotificationType.EveryHour:
                    return 1;
                case NotificationType.Every2Hour:
                    return 2;
                case NotificationType.Every4Hour:
                    return 4;
                case NotificationType.Every6Hour:
                    return 6;
                case NotificationType.Every8Hour:
                    return 8;
                default: return 100;
            }
        }

        private async Task<int> MatchOffers(IEnumerable<Filter> filters)
        {
            foreach (var filter in filters)
            {
                var matchCriteriaOffers = await _offerRepository
                    .FindByAsync(o =>
                        o.CityId == filter.CityId &&
                        o.Deal == filter.Deal &&
                        o.Price >= filter.PriceFrom &&
                        o.Price <= filter.PriceTo);

                if (matchCriteriaOffers.AnyAndNotNull())
                {
                    var offerFilters = matchCriteriaOffers.Select(o => new OfferFilter
                    {
                        FilterId = filter.Id,
                        OfferId = o.Id,
                        Sent = false
                    }).ToList();

                    var oldForThisFilter = (await _offerFilterRepository
                        .FindByAsync(of => of.FilterId == filter.Id))
                        .ToList();

                    var newestFrom = offerFilters
                            .Where(p => oldForThisFilter
                            .All(p2 => p2.OfferId != p.OfferId))
                            .ToList();

                    if (newestFrom.AnyAndNotNull())
                    {
                        _offerFilterRepository.BulkInsert(newestFrom, true);
                    }
                }
            }

            return 0;
        }
    }
}