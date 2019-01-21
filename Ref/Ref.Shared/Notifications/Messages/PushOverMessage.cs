namespace Ref.Shared.Notifications.Messages
{
    public class PushOverMessage
    {
        public PushOverMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public string Title { get; private set; }
        public string Message { get; private set; }
    }
}