using Microsoft.Extensions.Logging;
using Ref.Data.Repositories;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Cleaner.Core
{
    public class CleanService
    {
        private readonly ILogger<CleanService> _logger;

        private readonly IAppProvider _appProvider;

        private readonly IOfferRepository _offerRepository;

        private readonly IPushOverNotification _pushOverNotification;

        public CleanService(
            ILogger<CleanService> logger,
            IAppProvider appProvider,
            IOfferRepository offerRepository,
            IPushOverNotification pushOverNotification)
        {
            _logger = logger;
            _appProvider = appProvider;
            _offerRepository = offerRepository;
            _pushOverNotification = pushOverNotification;
        }

        public async Task<int> Clean()
        {
            var successTries = 0;

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    var offersToDelete = await _offerRepository
                        .FindByAsync(o => o.DateAdded < DateTime.Now.AddDays(-14));

                    if (offersToDelete.AnyAndNotNull())
                    {
                        offersToDelete = offersToDelete.Take(1000);

                        _offerRepository.BulkDelete(
                            offersToDelete
                            .Select(o => o.Id)
                            .ToList());

                        _logger.LogTrace($"{offersToDelete.Count()} offers deleted.");
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