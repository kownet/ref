using Microsoft.Extensions.Logging;
using Ref.Data.Repositories;
using Ref.Data.Views;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Sites;
using System;
using System.Linq;

namespace Ref.App.Core
{
    public class RefService
    {
        private readonly ILogger<RefService> _logger;

        private readonly IFilterProvider _filterProvider;
        private readonly ISite _site;
        private readonly IAdRepository _adRepository;
        private readonly IPushOverNotification _pushOverNotification;
        private readonly IEmailNotification _emailNotification;

        public RefService(
            ILogger<RefService> logger,
            IFilterProvider filterProvider,
            ISite site,
            IAdRepository adRepository,
            IPushOverNotification pushOverNotification,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _filterProvider = filterProvider;
            _site = site;
            _adRepository = adRepository;
            _pushOverNotification = pushOverNotification;
            _emailNotification = emailNotification;
        }

        public void Crawl()
        {
            try
            {
                var oldest = _adRepository.GetAll();

                var newest = _site.Search(_filterProvider);

                if (newest.AnyAndNotNull())
                {
                    _adRepository.SaveAll(newest);
                }

                var newestOne = newest.Where(p => oldest.All(p2 => p2.Id != p.Id));

                var ntfp = View.ForPushOver(newestOne);
                var ntfe = View.ForEmail(newestOne);

                _emailNotification.Send(ntfe.Title, ntfe.RawMessage, ntfe.HtmlMessage);

                _pushOverNotification.Send(ntfp.Title, ntfp.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _pushOverNotification.Send("Ref Error", ex.Message);
            }
        }
    }
}