using Microsoft.Extensions.Logging;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Data.Views;
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

namespace Ref.App.Standalone.Core
{
    public class RefStandaloneService
    {
        private readonly ILogger<RefStandaloneService> _logger;

        private readonly Func<SiteType, ISiteToScrapp> _siteAccessor;
        private readonly IAppProvider _appProvider;

        private readonly IAdRepository _adRepository;
        private readonly IUserRepository _userRepository;

        private readonly IPushOverNotification _pushOverNotification;
        private readonly IEmailNotification _emailNotification;

        public RefStandaloneService(
            ILogger<RefStandaloneService> logger,
            Func<SiteType, ISiteToScrapp> siteAccessor,
            IAppProvider appProvider,
            IAdRepository adRepository,
            IUserRepository userRepository,
            IPushOverNotification pushOverNotification,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _siteAccessor = siteAccessor;
            _appProvider = appProvider;
            _adRepository = adRepository;
            _userRepository = userRepository;
            _pushOverNotification = pushOverNotification;
            _emailNotification = emailNotification;
        }

        public async Task<int> Crawl()
        {
            var successTries = 0;

            var availableSites = _appProvider.Sites().Select(s => (SiteType)s);

            #region Search per client filters from flat JSON files
            var clients = _userRepository.GetAll()
                .Where(c => c.IsWorkingTime)
                .Where(c => c.IsActive);

            while (successTries < _appProvider.SuccessTries())
            {
                try
                {
                    if (clients.AnyAndNotNull())
                    {
                        foreach (var client in clients.Where(c => !c.IsChecked))
                        {
                            var oldest = _adRepository.GetAll(client.Code);

                            var newestAll = new List<Ad>();
                            var newest = new List<Ad>();
                            var filterName = string.Empty;
                            var filterDesc = string.Empty;

                            foreach (SiteType siteType in availableSites)
                            {
                                var result = _siteAccessor(siteType).Search(client.Filters);

                                if (result.WeAreBanned)
                                {
                                    _pushOverNotification.Send(
                                        Labels.BannedMsgTitle,
                                        Labels.BannedMsg(siteType.ToString()));
                                }

                                var newestFromSite = result.Advertisements
                                    .DistinctBy(p => p.Header);

                                filterName = result.FilterName;
                                filterDesc = result.FilterDesc;

                                newestAll.AddRange(newestFromSite);

                                var newestFrom = newestFromSite
                                    .Where(p => oldest.Where(t => t.SiteType == siteType)
                                    .All(p2 => p2.Id != p.Id));

                                newest.AddRange(newestFrom);

                                _logger.LogTrace(
                                    $"From {siteType.ToString()} collected {newestFromSite.Count()} records," +
                                    $" {newestFrom.Count()} new. Client '{client.Name}'.");
                            }

                            if (newestAll.AnyAndNotNull())
                            {
                                _adRepository.SaveAll(client.Code, newestAll);
                            }

                            if (newest.AnyAndNotNull())
                            {
                                if (client.Notification)
                                {
                                    var ntfe = View.ForEmail(newest, filterName, filterDesc);

                                    var email = _emailNotification.Send(
                                        ntfe.Title,
                                        ntfe.RawMessage,
                                        ntfe.HtmlMessage,
                                        new string[] { $"{client.Name} <{client.Email}>" });

                                    if (email.IsSuccess)
                                    {
                                        _logger.LogTrace($"Email to: {client.Email} sent.");
                                    }
                                    else
                                    {
                                        _pushOverNotification.Send(
                                            Labels.ErrorMsgTitle,
                                            Labels.ErrorEmailMsg(client.Email));
                                    }
                                }

                                if (_appProvider.AdminNotification())
                                {
                                    var ntfp = View.ForPushOver(newest, client.Email);

                                    _pushOverNotification.Send(ntfp.Title, ntfp.Message);
                                }
                            }

                            client.IsChecked = true;

                            Thread.Sleep(_appProvider.PauseTime());
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
            #endregion

            return 0;
        }
    }
}