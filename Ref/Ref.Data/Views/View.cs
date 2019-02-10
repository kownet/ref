using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Messages;
using Ref.Shared.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Data.Views
{
    public static class View
    {
        public static PushOverMessage ForPushOver(IEnumerable<Ad> records, string title = "")
        {
            var header = string.IsNullOrWhiteSpace(title) ? "" : $" - {title}";

            if (records.AnyAndNotNull())
            {
                var sb = new StringBuilder();

                var grouped = records.GroupBy(r => r.SiteType);

                foreach (var group in grouped)
                {
                    var site = group.Key.ToString();

                    var list = group.Select(g => g);

                    var count = 0;

                    if (list.AnyAndNotNull())
                    {
                        count = list.Count();

                        sb.AppendLine($"{site} [{list.Count()}]");
                    }
                }

                sb.AppendLine(Labels.RecordsFoundPushoverContent);

                return new PushOverMessage($"{Labels.RecordsFoundTitle}{header}", sb.ToString());
            }
            else
            {
                return new PushOverMessage($"{Labels.NoNewRecordsMsgTitle}{header}", Labels.NoNewRecordsMsgContent);
            }
        }

        public static EmailMessage ForEmail(IEnumerable<Ad> records, string title = "", string filterDesc = "")
        {
            var header = string.IsNullOrWhiteSpace(title) ? "" : $" - {title}";

            if (records.AnyAndNotNull())
            {
                var sbRaw = new StringBuilder();
                var sbHtml = new StringBuilder();

                var grouped = records.GroupBy(r => r.SiteType);

                if(!string.IsNullOrWhiteSpace(filterDesc))
                {
                    sbRaw.AppendLine(filterDesc);
                    sbHtml.AppendLine($"<i>{filterDesc}</i><br><br>");
                }

                foreach (var group in grouped)
                {
                    var site = group.Key.ToString();

                    var list = group.Select(g => g);

                    var count = 0;

                    if (list.AnyAndNotNull())
                        count = list.Count();

                    sbRaw.AppendLine($"{site} [{count}]:");
                    sbHtml.AppendLine($"<strong>{site} [{count}]:</strong><br>");

                    foreach (var element in list)
                    {
                        var msgRaw = $"{element.Price} - {element.Url} - {element.Header}";
                        sbRaw.AppendLine(msgRaw);

                        var msgHtml = $"{element.Price} [zł] - <a href=\"{element.Url}\">{element.Header}</a><br>";
                        sbHtml.AppendLine(msgHtml);
                    }
                }

                return new EmailMessage($"{Labels.RecordsFoundTitle}{header}", sbRaw.ToString(), sbHtml.ToString());
            }
            else
            {
                return new EmailMessage($"{Labels.NoNewRecordsMsgTitle}{header}", Labels.NoNewRecordsMsgContent);
            }
        }
    }
}