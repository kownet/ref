using Ref.Data.Models;

namespace Ref.Sites.QueryStrings
{
    public interface IQueryString
    {
        string Get(Filter filter);
        string Get(City city, DealType dealType);
    }
}