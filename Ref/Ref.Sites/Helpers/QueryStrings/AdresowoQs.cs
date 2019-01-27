using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class AdresowoQs : IQs
    {
        private readonly IFilterProvider _filter;

        public AdresowoQs(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Adresowo, _filter);
            var deal = FilterResolver.Deal(SiteType.Adresowo, _filter);
            var market = FilterResolver.Market(SiteType.Adresowo, _filter);

            var result =
                $"https://adresowo.pl/{type}/{_filter.Location()}/";

            if (_filter.PriceFrom() != 0)
                result = result + $"p{_filter.PriceFrom() / 10000}"; /// TODO: price calculate

            if (_filter.PriceTo() != 0)
                result = result + $"-{_filter.PriceTo() / 10000}_";

            if (_filter.FlatAreaFrom() != 0)
                result = result + $"a{_filter.FlatAreaFrom()}";

            if (_filter.FlatAreaTo() != 0)
                result = result + $"-{_filter.FlatAreaTo()}";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"_{market}";

            if (_filter.Newest() == 1)
                result = result + $"_lod";

            return result;
        }
    }
}