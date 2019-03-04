namespace Ref.Shared.Providers
{
    public interface IAppNotifierProvider : IAppBaseProvider
    {
    }

    public class AppNotifierProvider : AppBaseProvider, IAppNotifierProvider
    {
        public AppNotifierProvider(
            string address,
            string sender,
            string replyto,
            string pausetime,
            string adminnotification,
            string successTries,
            string appId)
            : base(address, sender, replyto, pausetime, adminnotification, successTries, appId)
        {
        }
    }
}