using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class OlxQs : IQs
    {
        private readonly IFilterProvider _filter;

        public OlxQs(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Olx, _filter);
            var deal = FilterResolver.Deal(SiteType.Olx, _filter);
            var market = FilterResolver.Market(SiteType.Olx, _filter);

            var result =
                $"https://www.olx.pl/nieruchomosci/{type}/{deal}/{_filter.Location()}/?";

            var divider = "search%5B";

            if (_filter.PriceFrom() != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={_filter.PriceFrom()}&";

            if (_filter.PriceTo() != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={_filter.PriceTo()}&";

            if (_filter.FlatAreaFrom() != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={_filter.FlatAreaFrom()}&";

            if (_filter.FlatAreaTo() != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={_filter.FlatAreaTo()}&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";

            if (_filter.Newest() == 1)
                result = result + $"{divider}Border%5D=created_at%3Adesc&";

            result = result + $"{divider}private_business%5D=private&";

            return result;
        }
    }
}