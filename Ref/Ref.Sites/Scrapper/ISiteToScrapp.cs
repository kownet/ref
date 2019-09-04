using Ref.Data.Components;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper
{
    public interface ISiteToScrapp
    {
        ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter);
    }
}