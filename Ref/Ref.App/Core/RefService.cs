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
        private readonly IAppProvider _appProvider;

        private readonly IAdRepository _adRepository;
        private readonly IClientRepository _clientRepository;

        private readonly IPushOverNotification _pushOverNotification;
        private readonly IEmailNotification _emailNotification;

        public RefService(
            ILogger<RefService> logger,
            Func<SiteType, ISite> siteAccessor,
            IAppProvider appProvider,
            IAdRepository adRepository,
            IClientRepository clientRepository,
            IPushOverNotification pushOverNotification,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _adRepository = adRepository;
            _clientRepository = clientRepository;
            _pushOverNotification = pushOverNotification;
            _emailNotification = emailNotification;
        }

        public void Crawl()
        {
            /// TODO: filter name to email

            try
            {
                var clients = _clientRepository.GetAll();

                if (clients.AnyAndNotNull())
                {
                    foreach (var client in clients)
                    {
                        var oldest = _adRepository.GetAll(client.Code);

                        if(oldest.AnyAndNotNull())
                        {
                            var newestAll = new List<Ad>();
                            var newest = new List<Ad>();

                            foreach (SiteType siteType in Enum.GetValues(typeof(SiteType)))
                            {
                                var newestFromSite = _siteAccessor(siteType).Search(client.Filters);

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
                                _adRepository.SaveAll(client.Code, newestAll);
                            }

                            if (newest.AnyAndNotNull())
                            {
                                var ntfe = View.ForEmail(newest);

                                _emailNotification.Send(
                                    ntfe.Title,
                                    ntfe.RawMessage,
                                    ntfe.HtmlMessage,
                                    new string[] { $"{client.Name} <{client.Email}>" });
                            }

                            var ntfp = View.ForPushOver(newest);

                            _pushOverNotification.Send(ntfp.Title, ntfp.Message);
                        }

                        Thread.Sleep(_appProvider.PauseTime());
                    }
                }
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