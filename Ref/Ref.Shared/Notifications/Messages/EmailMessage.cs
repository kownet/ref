namespace Ref.Shared.Notifications.Messages
{
    public class EmailMessage
    {
        public EmailMessage(string title, string message)
        {
            Title = title;
            RawMessage = message;
        }

        public EmailMessage(string title, string message, string htmlMessage)
        {
            Title = title;
            RawMessage = message;
            HtmlMessage = htmlMessage;
        }

        public string Title { get; private set; }
        public string RawMessage { get; private set; }
        public string HtmlMessage { get; set; }
    }
}