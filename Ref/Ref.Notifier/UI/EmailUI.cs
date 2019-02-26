using Ref.Data.Components;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Notifier.UI
{
    public class EmailUI
    {
        public EmailUI()
        {
            Filters = new Dictionary<string, IEnumerable<MailReportOffer>>();
        }

        public Dictionary<string, IEnumerable<MailReportOffer>> Filters { get; set; }

        public EmailMessage Prepare()
        {
            var sbRaw = new StringBuilder();
            var sbHtml = new StringBuilder();

            foreach (var filter in Filters)
            {
                sbRaw.AppendLine($"Filtr: {filter.Key}:");
                sbHtml.AppendLine($"</br><strong>Filtr: {filter.Key}:<strong></br></br>");

                var siteGrouped = filter.Value.GroupBy(s => s.SiteType);

                foreach (var site in siteGrouped)
                {
                    var header = $"{site.Key.ToString()} - [{site.ToList().Count}]";

                    sbRaw.AppendLine($"Strona: {header}:");
                    sbHtml.AppendLine($"</br><i>Strona: {header}:</i></br></br>");

                    var offers = site.OrderBy(o => o.Price).ToList();

                    foreach (var offer in offers)
                    {
                        sbRaw.AppendLine($"{offer.Price} - {offer.Url} - {offer.Header}");
                        sbHtml.AppendLine($"{offer.Price} [zł] - <a href=\"{offer.Url}\">{offer.Header}</a><br>");
                    }
                }
            }

            return new EmailMessage(
                "REF",
                sbRaw.ToString(),
                sbHtml.ToString());
        }

        public bool CanBeSend => Filters.AnyAndNotNull();
    }
}