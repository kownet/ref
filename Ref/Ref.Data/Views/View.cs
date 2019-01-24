using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Data.Views
{
    public static class View
    {
        public static PushOverMessage ForPushOver(IEnumerable<Ad> records)
        {
            if(records.AnyAndNotNull())
            {
                var isare = records.Count() > 1 ? $"are {records.Count()}" : $"is {records.Count()}";
                var s = records.Count() > 1 ? "s" : string.Empty;

                var title = $"There {isare} new record{s}";

                var sb = new StringBuilder();

                foreach (var record in records)
                {
                    var msg = $"{record.Price} - <a href=\"{record.Url}\">{record.Header}</a>";

                    sb.AppendLine(msg);
                }

                return new PushOverMessage(title, sb.ToString());
            }
            else
            {
                return new PushOverMessage("There are no new records.", "Maybe next time.");
            }
        }

        public static EmailMessage ForEmail(IEnumerable<Ad> records)
        {
            if (records.AnyAndNotNull())
            {
                var isare = records.Count() > 1 ? $"are {records.Count()}" : $"is {records.Count()}";
                var s = records.Count() > 1 ? "s" : string.Empty;

                var title = $"There {isare} new record{s}";

                var sbRaw = new StringBuilder();

                foreach (var record in records)
                {
                    var msg = $"{record.Price} - {record.Url} - {record.Header}";

                    sbRaw.AppendLine(msg);
                }

                var sbHtml = new StringBuilder();

                foreach (var record in records)
                {
                    var msg = $"{record.Price} - <a href=\"{record.Url}\">{record.Header}</a> [{record.Area}]<br>";

                    sbHtml.AppendLine(msg);
                }

                return new EmailMessage(title, sbRaw.ToString(), sbHtml.ToString());
            }
            else
            {
                return new EmailMessage("There are no new records.", "Maybe next time.");
            }
        }
    }
}