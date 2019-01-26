using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public interface IQs
    {
        string Get(IFilterProvider filter);
    }
}