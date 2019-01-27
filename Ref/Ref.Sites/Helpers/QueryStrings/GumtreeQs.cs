using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class GumtreeQs : IQs
    {
        private readonly IFilterProvider _filter;

        public GumtreeQs(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Gumtree, _filter);
            var deal = FilterResolver.Deal(SiteType.Gumtree, _filter);
            var market = FilterResolver.Market(SiteType.Gumtree, _filter);

            var result =
                $"";

            return result;
        }
    }
}