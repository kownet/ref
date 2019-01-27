using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class GratkaQs : IQs
    {
        private readonly IFilterProvider _filter;

        public GratkaQs(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Gratka, _filter);
            var deal = FilterResolver.Deal(SiteType.Gratka, _filter);
            var market = FilterResolver.Market(SiteType.Gratka, _filter);

            var result =
                $"";

            return result;
        }
    }
}