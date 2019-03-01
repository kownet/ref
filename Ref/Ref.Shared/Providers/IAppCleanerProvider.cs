namespace Ref.Shared.Providers
{
    public interface IAppCleanerProvider : IAppBaseProvider
    {
        int DaysToLive();
    }

    public class AppCleanerProvider : AppBaseProvider, IAppCleanerProvider
    {
        private readonly string _daysToLive;

        public AppCleanerProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId,
            string daysToLive)
            : base(address, sender, replyto, pausetime, adminnotification, successTries, appId)
        {
            _daysToLive = daysToLive;
        }

        public int DaysToLive() => int.Parse(_daysToLive);
    }
}