using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class GratkaQueryString : IQueryString
    {
        public string Get(Filter _filter)
        {
            var type = FilterResolver.Type(SiteType.Gratka, _filter);
            var deal = FilterResolver.Deal(SiteType.Gratka, _filter);
            var market = FilterResolver.Market(SiteType.Gratka, _filter);

            var result =
                $"https://gratka.pl/nieruchomosci/{type}/{_filter.LocationRaw}/{deal}?";

            if (_filter.PriceFrom != 0)
                result = result + $"cena-calkowita:min={_filter.PriceFrom}";

            if (_filter.PriceTo != 0)
                result = result + $"&cena-calkowita:max={_filter.PriceTo}";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"&powierzchnia-w-m2:min={_filter.FlatAreaFrom}";

            if (_filter.FlatAreaTo != 0)
                result = result + $"&powierzchnia-w-m2:max={_filter.FlatAreaTo}";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"&rynek={market}";

            if (_filter.Newest == 1)
                result = result + $"&data-dodania-search=ostatnich-24h&sort=newest";

            return result;
        }
    }
}