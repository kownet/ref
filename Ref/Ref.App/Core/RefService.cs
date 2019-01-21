using Microsoft.Extensions.Options;
using Ref.App.Options;
using Ref.Data.Repositories;
using Ref.Data.Views;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using Ref.Sites;
using System;
using System.Linq;

namespace Ref.App.Core
{
    public class RefService
    {
        private readonly IOptions<FilterConfiguration> _configuration;
        private readonly ISite _site;
        private readonly IAdRepository _adRepository;
        private readonly IPushOverNotification _pushOverNotification;

        public RefService(
            IOptions<FilterConfiguration> configuration,
            ISite site,
            IAdRepository adRepository,
            IPushOverNotification pushOverNotification)
        {
            _configuration = configuration;
            _site = site;
            _adRepository = adRepository;
            _pushOverNotification = pushOverNotification;
        }

        public void Crawl()
        {
            try
            {
                var oldest = _adRepository.GetAll();

                var newest = _site.Search(
                    _configuration.Value.Type,
                    _configuration.Value.Deal,
                    _configuration.Value.Location,
                    _configuration.Value.FlatAreaFrom,
                    _configuration.Value.FlatAreaTo);

                if (newest.AnyAndNotNull())
                {
                    _adRepository.SaveAll(newest);
                }

                var newestOne = newest.Where(p => oldest.All(p2 => p2.Id != p.Id));

                if (newestOne.AnyAndNotNull())
                {
                    var ntf = View.ForPushOver(newestOne);

                    _pushOverNotification.Send(ntf.Title, ntf.Message);
                }
            }
            catch (Exception ex)
            {
                _pushOverNotification.Send("Ref Error", ex.Message);
            }
        }
    }
}