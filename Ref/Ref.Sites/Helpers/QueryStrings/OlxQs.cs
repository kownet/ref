using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class OlxQs : IQs
    {
        public string Get(IFilterProvider filter)
        {
            var type = FilterResolver.Type(SiteType.Olx, filter);
            var deal = FilterResolver.Deal(SiteType.Olx, filter);
            var market = FilterResolver.Market(SiteType.Olx, filter);

            var result =
                $"https://www.olx.pl/nieruchomosci/{type}/{deal}/{filter.Location()}/?";

            var divider = "search%5B";

            if (filter.PriceFrom() != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={filter.PriceFrom()}&";

            if (filter.PriceTo() != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={filter.PriceTo()}&";

            if (filter.FlatAreaFrom() != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={filter.FlatAreaFrom()}&";

            if (filter.FlatAreaTo() != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={filter.FlatAreaTo()}&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";

            if (filter.Newest() == 1)
                result = result + $"{divider}Border%5D=created_at%3Adesc";

            return result;
        }
    }
}