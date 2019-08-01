using Microsoft.Extensions.Logging;
using Ref.Data.Components;
using Ref.Data.Repositories;
using Ref.Notifier.UI;
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

        private readonly IAppNotifierProvider _appProvider;

        private readonly IOfferFilterRepository _offerFilterRepository;

        private readonly IMailReport _mailReport;

        private readonly IEmailNotification _emailNotification;
        private readonly IPushOverNotification _pushOverNotification;

        public NotifierService(
            ILogger<NotifierService> logger,
            IAppNotifierProvider appProvider,
            IOfferFilterRepository offerFilterRepository,
            IMailReport mailReport,
            IEmailNotification emailNotification,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _offerFilterRepository = offerFilterRepository;
            _mailReport = mailReport;
            _emailNotification = emailNotification;
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
                            .Select(gr => new { Email = gr.Key, Token = gr.First().Token, Filters = gr.ToList() })
                            .ToList();

                        foreach (var user in groupedForClient)
                        {
                            var email = new EmailUI(user.Token);

                            var filterNames = user.Filters.Select(f => f.Filter);

                            var mailTitle = FiltersTopicResolver.GetAbbreviation(filterNames);

                            foreach (var filter in user.Filters)
                            {
                                var offersForEachFilter = await _mailReport.GetAllOffersForFilterAsync(filter.FilterId);

                                if (offersForEachFilter.AnyAndNotNull())
                                {
                                    email.Filters.Add(filter.Filter, offersForEachFilter);
                                }
                            }

                            var emailToSend = email.Prepare(mailTitle);

                            if (email.CanBeSend)
                            {
                                var emailResponse = _emailNotification.Send(
                                    emailToSend.Title,
                                    emailToSend.RawMessage,
                                    emailToSend.HtmlMessage,
                                    new string[] { user.Email });

                                if (emailResponse.IsSuccess)
                                {
                                    await _mailReport.UpdateFiltersAsSentAsync(user.Filters.Select(f => f.FilterId));

                                    var msg = $"Mail to: {user.Email} with result sent.";

                                    _logger.LogError(msg);

                                    if(_appProvider.AdminNotification())
                                    {
                                        _pushOverNotification.Send(
                                        Labels.SuccessEmailMsgTitle,
                                        msg);
                                    }
                                }
                                else
                                {
                                    var msg = $"Mail to: {user.Email} with result NOT sent. Message: {emailResponse.Message}";

                                    _logger.LogError(msg);

                                    _pushOverNotification.Send(
                                        Labels.ErrorEmailMsgTitle,
                                        msg);
                                }
                            }
                            else
                            {
                                var msg = $"Nothing to send for: {user.Email}.";

                                _logger.LogError(msg);
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