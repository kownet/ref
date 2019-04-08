using Ref.Data.Components;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Messages;
using Ref.Shared.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Notifier.UI
{
    public class EmailUI
    {
        public EmailUI(string token)
        {
            Filters = new Dictionary<string, IEnumerable<MailReportOffer>>();
            Token = token;
        }

        public Dictionary<string, IEnumerable<MailReportOffer>> Filters { get; set; }
        public string Token { get; private set; }

        public EmailMessage Prepare()
        {
            var sbRaw = new StringBuilder();
            var sbHtml = new StringBuilder();

            var sbHeader =
                $"Twoje ustawienia dostępne <a href=\"https://app.pewnemieszkanie.pl/index.html?guid={Token}\">tutaj.</a><br><br>";

            var sbFooter =
                $"<br><br>-----" + 
                $"<br>Udanych zakupów życzy zespół: <a href=\"https://pewnemieszkanie.pl/\">PewneMieszkanie.pl</a>" +
                $"<br>Zapraszamy na nasz <a href=\"https://www.facebook.com/PewneMieszkanie/\">Facebook</a>.";

            sbHtml.AppendLine(sbHeader);

            foreach (var filter in Filters)
            {
                sbRaw.AppendLine($"Filtr: {filter.Key}:");
                sbHtml.AppendLine($"<br><strong>Filtr: {filter.Key}:<strong><br><br>");

                var siteGrouped = filter.Value.GroupBy(s => s.Site);

                foreach (var site in siteGrouped)
                {
                    var header = $"{site.Key.ToString()} - [{site.ToList().Count}]";

                    sbRaw.AppendLine($"Strona: {header}:");
                    sbHtml.AppendLine($"<br><i>Strona: {header}:</i><br><br>");

                    var offers = site.OrderBy(o => o.Price).ToList();

                    foreach (var offer in offers)
                    {
                        sbRaw.AppendLine($"{offer.Price} - {offer.Url} - {offer.Header}");
                        sbHtml.AppendLine($"{offer.Price} [zł] - <a href=\"{offer.Url}\">{offer.Header}</a><br>");
                    }
                }
            }

            sbHtml.AppendLine(sbFooter);

            return new EmailMessage(
                $"{Labels.RecordsFoundTitle}",
                sbRaw.ToString(),
                sbHtml.ToString());
        }

        public bool CanBeSend => Filters.AnyAndNotNull();
    }
}