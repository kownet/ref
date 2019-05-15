using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;

namespace Ref.Sites.QueryStrings
{
    public interface IQueryString
    {
        string Get(City city, DealType dealType, District district = null);
        string Get(UserSubscriptionFilter userSubscriptionFilter);
    }
}