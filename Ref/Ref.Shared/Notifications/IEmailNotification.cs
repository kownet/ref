using Newtonsoft.Json;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications.Payloads;
using Ref.Shared.Providers;
using Ref.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Shared.Notifications
{
    public class EmailResponse
    {
        public string Message { get; set; }

        public bool IsSuccess => string.IsNullOrWhiteSpace(Message);
    }

    public interface IEmailNotification
    {
        EmailResponse Send(string title, string rawMessage, string htmlMessage, string[] recipients);
    }

    public class EmailNotification : IEmailNotification
    {
        private readonly IEmailProvider _emailProvider;
        private readonly ISenderProvider _senderProvider;

        public EmailNotification(
            IEmailProvider emailProvider,
            ISenderProvider senderProvider)
        {
            _emailProvider = emailProvider;
            _senderProvider = senderProvider;
        }

        public EmailResponse Send(string title, string rawMessage, string htmlMessage, string[] recipients)
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
                    Sender = _senderProvider.Sender(),
                    Subject = title,
                    Text = rawMessage,
                    Html = htmlMessage,
                    Headers = new List<EmailPayloadCustomHeader>
                    {
                        new EmailPayloadCustomHeader
                        {
                            Header = "Reply-To",
                            Value = _senderProvider.ReplyTo()
                        }
                    }
                };

                using (var request = new HttpRequestMessage(HttpMethod.Post, "email/send"))
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient() { BaseAddress = new Uri(_emailProvider.Host()) })
                    {
                        var response = Task.Run(() => httpClient.SendAsync(request)).Result;

                        var error = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

                        try
                        {
                            var status = response.EnsureSuccessStatusCode();
                        }
                        catch (Exception ex)
                        {
                            return new EmailResponse
                            {
                                Message = ex.Message
                            };
                        }
                    }
                }

                return new EmailResponse();
            }

            return new EmailResponse
            {
                Message = Labels.EmptyEmailError
            };
        }
    }
}