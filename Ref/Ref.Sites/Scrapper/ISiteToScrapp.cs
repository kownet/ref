using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper
{
    public interface ISiteToScrapp
    {
        ScrappResponse Scrapp(City city, DealType dealType, District district);
        ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter);
    }
}