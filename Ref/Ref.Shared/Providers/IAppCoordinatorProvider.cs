namespace Ref.Shared.Providers
{
    public interface IAppCoordinatorProvider : IAppBaseProvider
    {
    }

    public class AppCoordinatorProvider : AppBaseProvider, IAppCoordinatorProvider
    {
        public AppCoordinatorProvider(
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