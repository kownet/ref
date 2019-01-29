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

            var code = FilterResolver.Code(_filter);

            var houseOrFlat = _filter.Type() == 0 ? "mieszkanie" : "dom";

            var page = 1;

            var result =
                $"https://www.gumtree.pl/{type}{deal}/{_filter.Location()}/{houseOrFlat}/{code}{page}?";

            var pFrom = _filter.PriceFrom() == 0 ? string.Empty : _filter.PriceFrom().ToString();
            var pTo = _filter.PriceTo() == 0 ? string.Empty : _filter.PriceTo().ToString();

            result = result + $"pr={pFrom},{pTo}";

            if (_filter.Newest() == 1)
                result = result + $"&df=ownr&sort=dt&order=desc";

            return result;
        }
    }
}