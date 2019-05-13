using Microsoft.Extensions.Logging;
using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Coordinator.Core
{
    public class CoordinatorService
    {
        private readonly ILogger<CoordinatorService> _logger;

        private readonly IAppCoordinatorProvider _appProvider;

        private readonly IOfferRepository _offerRepository;
        private readonly IOfferFilterRepository _offerFilterRepository;
        private readonly IUserRepository _userRepository;

        private readonly IUserSubscriptionReport _userSubscriptionReport;

        private readonly IPushOverNotification _pushOverNotification;

        public CoordinatorService(
            ILogger<CoordinatorService> logger,
            IAppCoordinatorProvider appProvider,
            IOfferRepository offerRepository,
            IOfferFilterRepository offerFilterRepository,
            IUserRepository userRepository,
            IUserSubscriptionReport userSubscriptionReport,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _offerRepository = offerRepository;
            _offerFilterRepository = offerFilterRepository;
            _userRepository = userRepository;
            _userSubscriptionReport = userSubscriptionReport;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Manage()
        {
            var successTries = 0;

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    var filtersToCheck = (await _userSubscriptionReport.GetAllActiveAsync())
                        .Where(u => !u.DemoPassed);

                    var grouped = filtersToCheck.GroupBy(f => f.Notification);

                    foreach (var group in grouped)
                    {
                        var oldFiltersToCheckGroup = group
                            .Where(f =>
                                f.LastCheckedAt.Value <= DateTime.Now.AddHours(-ResolveTimeInterval(group.Key)));

                        if (oldFiltersToCheckGroup.AnyAndNotNull())
                        {
                            _logger.LogTrace($"There are {oldFiltersToCheckGroup.Count()} filters to check.");

                            await MatchOffers(oldFiltersToCheckGroup);

                            _logger.LogTrace("Filters checked");
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

        private async Task<int> MatchOffers(IEnumerable<UserSubscription> filters)
        {
            foreach (var filter in filters)
            {
                var userRegisterDate = (await _userRepository.FindByAsync(u => u.Id == filter.UserId))
                    .FirstOrDefault().RegisteredAt;

                Expression<Func<Offer, bool>> predicate = (o =>
                    o.CityId == filter.CityId &&
                    o.Deal == filter.Deal &&
                    o.DateAdded >= userRegisterDate &&
                    o.IsScrapped);

                if (filter.PriceFrom.HasValue)
                    predicate = predicate.And(o => o.Price >= filter.PriceFrom.Value);

                if (filter.PriceTo.HasValue)
                    predicate = predicate.And(o => o.Price <= filter.PriceTo.Value);

                if (filter.FlatAreaFrom.HasValue)
                    predicate = predicate.And(o => o.Area >= filter.FlatAreaFrom.Value);

                if (filter.FlatAreaTo.HasValue)
                    predicate = predicate.And(o => o.Area <= filter.FlatAreaTo.Value);

                if (filter.PricePerMeterFrom.HasValue)
                    predicate = predicate.And(o => o.PricePerMeter >= filter.PricePerMeterFrom.Value);

                if (filter.PricePerMeterTo.HasValue)
                    predicate = predicate.And(o => o.PricePerMeter <= filter.PricePerMeterTo.Value);

                if (filter.DistrictId.HasValue)
                {
                    predicate = predicate.And(o => o.DistrictId.HasValue);
                    predicate = predicate.And(o => o.DistrictId == filter.DistrictId.Value);
                }

                var matchCriteriaOffers = await _offerRepository
                    .FindByAsync(predicate);

                if (matchCriteriaOffers.AnyAndNotNull())
                {
                    _logger.LogTrace($"There are {matchCriteriaOffers.Count()} offers for filter {filter.Id}.");

                    var matchedOfferByShouldContain = new List<Offer>();

                    if (!string.IsNullOrWhiteSpace(filter.ShouldContain))
                    {
                        if (filter.ShouldContain.Contains(Statics.Separator))
                        {
                            var splittedPhrase = filter.ShouldContain.Split(Statics.Separator).Select(p => WebUtility.HtmlDecode(p.Trim()));

                            foreach (var matched in matchCriteriaOffers)
                            {
                                if (!string.IsNullOrWhiteSpace(matched.Content) && !string.IsNullOrWhiteSpace(matched.Header))
                                {
                                    if (splittedPhrase.Any(matched.Content.Contains)
                                        || splittedPhrase.Any(matched.Header.ToLowerInvariant().Contains))
                                    {
                                        _logger.LogTrace($"Offer {matched.Id} contain at least one from phrase {filter.ShouldContain}.");

                                        matchedOfferByShouldContain.Add(matched);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var matched in matchCriteriaOffers)
                            {
                                if (!string.IsNullOrWhiteSpace(matched.Content) && !string.IsNullOrWhiteSpace(matched.Header))
                                {
                                    if (matched.Content.Contains(WebUtility.HtmlDecode(filter.ShouldContain.Trim()))
                                        || matched.Header.ToLowerInvariant().Contains(WebUtility.HtmlDecode(filter.ShouldContain.Trim())))
                                    {
                                        _logger.LogTrace($"Offer {matched.Id} contain at least one from phrase {filter.ShouldContain}.");

                                        matchedOfferByShouldContain.Add(matched);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        matchedOfferByShouldContain.AddRange(matchCriteriaOffers);
                    }

                    if (!string.IsNullOrWhiteSpace(filter.ShouldNotContain))
                    {
                        if (filter.ShouldNotContain.Contains(Statics.Separator))
                        {
                            var splittedPhrase = filter.ShouldNotContain.Split(Statics.Separator).Select(p => WebUtility.HtmlDecode(p.Trim()));

                            foreach (var matched in matchedOfferByShouldContain)
                            {
                                if (!string.IsNullOrWhiteSpace(matched.Content) && !string.IsNullOrWhiteSpace(matched.Header))
                                {
                                    if (splittedPhrase.Any(matched.Content.Contains)
                                        || splittedPhrase.Any(matched.Header.ToLowerInvariant().Contains))
                                    {
                                        _logger.LogTrace($"Offer {matched.Id} contain at least one from phrase {filter.ShouldNotContain}.");

                                        matched.ToDelete = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var matched in matchedOfferByShouldContain)
                            {
                                if (!string.IsNullOrWhiteSpace(matched.Content) && !string.IsNullOrWhiteSpace(matched.Header))
                                {
                                    if (matched.Content.Contains(WebUtility.HtmlDecode(filter.ShouldNotContain.Trim())) 
                                        || matched.Header.ToLowerInvariant().Contains(WebUtility.HtmlDecode(filter.ShouldNotContain.Trim())))
                                    {
                                        _logger.LogTrace($"Offer {matched.Id} contain at least one from phrase {filter.ShouldNotContain}.");

                                        matched.ToDelete = true;
                                    }
                                }
                            }
                        }
                    }

                    matchedOfferByShouldContain.RemoveAll(o => o.ToDelete);

                    var offerFilters = matchedOfferByShouldContain.Select(o => new OfferFilter
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

                        _logger.LogTrace($"Filter: {filter.Id} updated with {newestFrom.Count} offers.");
                    }
                }
            }

            return 0;
        }
    }
}