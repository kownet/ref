using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Data.Views;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using Ref.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ref.App.Core
{
    public class RefService
    {
        private readonly ILogger<RefService> _logger;

        private readonly Func<SiteType, ISite> _siteAccessor;
        private readonly IFilterProvider _filterProvider;
        private readonly IAppProvider _appProvider;
        private readonly IAdRepository _adRepository;
        private readonly IPushOverNotification _pushOverNotification;
        private readonly IEmailNotification _emailNotification;

        public RefService(
            ILogger<RefService> logger,
            Func<SiteType, ISite> siteAccessor,
            IFilterProvider filterProvider,
            IAppProvider appProvider,
            IAdRepository adRepository,
            IPushOverNotification pushOverNotification,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _filterProvider = filterProvider;
            _appProvider = appProvider;
            _adRepository = adRepository;
            _pushOverNotification = pushOverNotification;
            _emailNotification = emailNotification;
        }

        public void Crawl()
        {
            try
            {
                var oldest = _adRepository.GetAll();

                var newestAll = new List<Ad>();
                var newest = new List<Ad>();

                foreach (SiteType siteType in Enum.GetValues(typeof(SiteType)))
                {
                    var newestFromSite = _siteAccessor(siteType).Search(_filterProvider);

                    newestAll.AddRange(newestFromSite);

                    var newestFrom = newestFromSite
                        .Where(p => oldest.Where(t => t.SiteType == siteType)
                        .All(p2 => p2.Id != p.Id));

                    newest.AddRange(newestFrom);

                    _logger.LogTrace($"From {siteType.ToString()} collected {newestFromSite.Count()} records, {newestFrom.Count()} new.");

                    Thread.Sleep(_appProvider.PauseTime());
                }

                if (newestAll.AnyAndNotNull())
                {
                    _adRepository.SaveAll(newestAll);
                }

                if(newest.AnyAndNotNull())
                {
                    var ntfe = View.ForEmail(newest);

                    _emailNotification.Send(ntfe.Title, ntfe.RawMessage, ntfe.HtmlMessage);
                }

                var ntfp = View.ForPushOver(newest);

                _pushOverNotification.Send(ntfp.Title, ntfp.Message);
            }
            catch (Exception ex)
            {
                var msgException = $"Message: {ex.GetFullMessage()}, StackTrace: {ex.StackTrace}";

                _logger.LogError(msgException);
                _pushOverNotification.Send(Labels.ErrorMsgTitle, ex.GetFullMessage());
            }
        }
    }
}