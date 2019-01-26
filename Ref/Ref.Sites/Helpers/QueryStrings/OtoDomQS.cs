using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class OtoDomQs : IQs
    {
        public string Get(IFilterProvider filter)
        {
            var type = FilterResolver.Type(SiteType.OtoDom, filter);
            var deal = FilterResolver.Deal(SiteType.OtoDom, filter);
            var market = FilterResolver.Market(SiteType.OtoDom, filter);

            var result =
                $"https://www.otodom.pl/{deal}/{type}/{filter.Location()}/?";

            var divider = "search%5B";

            if (filter.PriceFrom() != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={filter.PriceFrom()}&";

            if (filter.PriceTo() != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={filter.PriceTo()}&";

            if (filter.FlatAreaFrom() != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={filter.FlatAreaFrom()}&";

            if (filter.FlatAreaTo() != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={filter.FlatAreaTo()}&";

            result = result + $"{divider}created_since%5D={filter.Newest()}&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";

            return result;
        }
    }
}