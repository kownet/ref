namespace Ref.Shared.Providers
{
    public interface ISenderProvider
    {
        string Sender();
        string ReplyTo();
    }

    public class SenderProvider : ISenderProvider
    {
        private readonly string _sender;
        private readonly string _replyTo;

        public SenderProvider(
            string sender,
            string replyTo)
        {
            _sender = sender;
            _replyTo = replyTo;
        }

        public string ReplyTo() => _replyTo;

        public string Sender() => _sender;
    }
}