using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ref.Shared.Notifications.Payloads
{
    public class EmailPayload
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("to")]
        public List<string> To { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("text_body")]
        public string Text { get; set; }

        [JsonProperty("html_body")]
        public string Html { get; set; }

        [JsonProperty("custom_headers")]
        public List<EmailPayloadCustomHeader> Headers { get; set; }
    }

    public class EmailPayloadCustomHeader
    {
        [JsonProperty("header")]
        public string Header { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}