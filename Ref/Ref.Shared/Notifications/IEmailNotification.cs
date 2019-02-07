using Newtonsoft.Json;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Payloads;
using Ref.Shared.Providers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Shared.Notifications
{
    public interface IEmailNotification
    {
        void Send(string title, string rawMessage, string htmlMessage, string[] recipients);
    }

    public class EmailNotification : IEmailNotification
    {
        private readonly IEmailProvider _emailProvider;
        private readonly IAppProvider _appProvider;

        public EmailNotification(
            IEmailProvider emailProvider,
            IAppProvider appProvider)
        {
            _emailProvider = emailProvider;
            _appProvider = appProvider;
        }

        public void Send(string title, string rawMessage, string htmlMessage, string[] recipients)
        {
            if (!string.IsNullOrWhiteSpace(title) &&
                (!string.IsNullOrWhiteSpace(rawMessage) || !string.IsNullOrWhiteSpace(htmlMessage)) &&
                recipients.AnyAndNotNull())
            {
                var to = new List<string>(recipients);

                var payload = new EmailPayload
                {
                    ApiKey = _emailProvider.ApiKey(),
                    To = to,
                    Sender = _appProvider.Sender(),
                    Subject = title,
                    Text = rawMessage,
                    Html = htmlMessage,
                    Headers = new List<EmailPayloadCustomHeader>
                    {
                        new EmailPayloadCustomHeader
                        {
                            Header = "Reply-To",
                            Value = _appProvider.ReplyTo()
                        }
                    }
                };

                if (!string.IsNullOrWhiteSpace(_appProvider.Bcc()))
                {
                    var bcc = new List<string>
                    {
                        _appProvider.Bcc()
                    };

                    payload.Bcc = bcc;
                }

                using (var request = new HttpRequestMessage(HttpMethod.Post, "email/send"))
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient() { BaseAddress = new Uri(_emailProvider.Host()) })
                    {
                        var response = Task.Run(() => httpClient.SendAsync(request)).Result;

                        var error = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }
    }
}