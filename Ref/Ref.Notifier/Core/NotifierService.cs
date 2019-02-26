using Microsoft.Extensions.Logging;
using Ref.Data.Components;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Notifier.Core
{
    public class NotifierService
    {
        private readonly ILogger<NotifierService> _logger;

        private readonly IAppProvider _appProvider;

        private readonly IOfferFilterRepository _offerFilterRepository;

        private readonly IMailReport _mailReport;

        private readonly IPushOverNotification _pushOverNotification;

        public NotifierService(
            ILogger<NotifierService> logger,
            IAppProvider appProvider,
            IOfferFilterRepository offerFilterRepository,
            IMailReport mailReport,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _offerFilterRepository = offerFilterRepository;
            _mailReport = mailReport;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Notify()
        {
            var successTries = 0;

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    var filtersResultNotSent = await _mailReport.GetAllAsync();

                    if (filtersResultNotSent.AnyAndNotNull())
                    {
                        var groupedForClient = filtersResultNotSent
                            .GroupBy(f => f.Email)
                            .Select(gr => new { Email = gr.Key, Filters = gr.ToList() })
                            .ToList();

                        foreach (var user in groupedForClient)
                        {
                            var email = user.Email;

                            foreach (var filter in user.Filters)
                            {
                                var offersForEachFilter = await _mailReport.GetAllOffersForFilterAsync(filter.FilterId);

                                if(offersForEachFilter.AnyAndNotNull())
                                {
                                    var siteGrouped = offersForEachFilter.GroupBy(s => s.SiteType);

                                    foreach (var site in siteGrouped)
                                    {
                                        var header = $"{site.Key.ToString()} - [{site.ToList().Count}]";


                                    }
                                }
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

                    Thread.Sleep(5 * 1000);
                }
            }

            return 0;
        }
    }
}