using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using Ref.Sites.Scrapper.Single;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Scrapper.Core
{
    public class ScrapperService
    {
        private readonly ILogger<ScrapperService> _logger;

        private readonly Func<SiteType, ISingleSiteToScrapp> _siteAccessor;
        private readonly IAppScrapperProvider _appProvider;

        private readonly IOfferRepository _offerRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IEventRepository _eventRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public ScrapperService(
            ILogger<ScrapperService> logger,
            Func<SiteType, ISingleSiteToScrapp> siteAccessor,
            IAppScrapperProvider appProvider,
            IOfferRepository offerRepository,
            ISiteRepository siteRepository,
            IEventRepository eventRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _offerRepository = offerRepository;
            _siteRepository = siteRepository;
            _eventRepository = eventRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Scrapp()
        {
            var successTries = 0;

            var sitesDefined = _appProvider.Sites().Select(s => (SiteType)s);

            var availableSites = (await _siteRepository.GetAllAsync())
                .Where(s => sitesDefined.Contains(s.Type))
                .Where(s => s.IsActive);

            while (true)
            {
                var dateSince = DateTime.Now.AddMinutes(-1);

                try
                {
                    foreach (var site in availableSites)
                    {
                        var toScrapp = await _offerRepository
                            .FindByAsync(o => !o.IsScrapped && !o.IsBadlyScrapped && o.Site == site.Type && o.DateAdded <= dateSince);

                        if (toScrapp.AnyAndNotNull())
                        {
                            _logger.LogTrace($"Found {toScrapp.Count()} for site {site.Type.ToString()}");

                            var chunked = toScrapp.Chunk(_appProvider.ChunkSize());

                            foreach (var chunk in chunked)
                            {
                                foreach (var offer in chunk)
                                {
                                    var result = _siteAccessor(site.Type).SingleScrapp(offer);

                                    if (result.Succeed)
                                    {
                                        offer.Content = result.Content.ToLowerInvariant().Truncate(3070);
                                        offer.IsScrapped = true;

                                        if (result.Floor.HasValue)
                                            offer.Floor = result.Floor.Value;

                                        if (result.Area.HasValue)
                                            offer.Area = result.Area.Value;

                                        if (result.Rooms.HasValue)
                                            offer.Rooms = result.Rooms.Value;

                                        if (result.PricePerMeter.HasValue)
                                            offer.PricePerMeter = result.PricePerMeter.Value;

                                        if (!string.IsNullOrWhiteSpace(result.Abstract))
                                            offer.Abstract = result.Abstract;

                                        offer.IsFromPrivate = result.IsFromPrivate;
                                        offer.IsFromAgency = result.IsFromAgency;

                                        await _offerRepository.UpdateAsync(offer);
                                    }

                                    if (result.IsDeleted || result.IsRedirected || !result.Succeed)
                                    {
                                        await _offerRepository.SetBadlyScrappedAsync(offer.Id);
                                    }

                                    if (_appProvider.EventUpdate())
                                    {
                                        try
                                        {
                                            await _eventRepository.Upsert(new Event
                                            {
                                                Type = EventType.Success,
                                                Category = (EventCategory)((int)site.Type + 100),
                                                Message = $"Succeed:{result.Succeed}"
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            var msgException = $"Message: {ex.GetFullMessage()}, StackTrace: {ex.StackTrace}";

                                            _logger.LogError(msgException);
                                        }
                                    }

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

                    if (successTries > _appProvider.SuccessTries())
                        Environment.Exit(-100);

                    var msgHeader = $"[no. {successTries}]";

                    var msgException = $"{msgHeader} Message: {ex.GetFullMessage()}, StackTrace: {ex.StackTrace}";

                    _logger.LogError(msgException);

                    if (_appProvider.AdminNotification())
                    {
                        _pushOverNotification.Send(
                            $"[{_appProvider.AppId()}]{Labels.ErrorMsgTitle}",
                            $"{msgHeader} {ex.GetFullMessage()}");
                    }

                    Thread.Sleep(_appProvider.PauseTime());
                }
            }
        }
    }
}