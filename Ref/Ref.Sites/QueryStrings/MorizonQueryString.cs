using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class MorizonQueryString : IQueryString
    {
        public string Get(City city, DealType dealType)
        {
            var type = "mieszkania";
            var deal = dealType == DealType.Sale ? "" : "do-wynajecia";
            var market = "2";
            var newest = 1;

            var domain = "https://www.morizon.pl";

            if (dealType == DealType.Rent)
            {
                domain = $"{domain}/do-wynajecia";
            }

            var result =
                $"{domain}/{type}/{city.NameRaw}/?";

            var divider = "ps%5B";

            result = result + $"{divider}date_filter%5D={newest}&";

            result = result + $"{divider}market_type%5D%5B0%5D={market}";

            return result;
        }

        public string Get(Filter _filter)
        {
            var type = FilterResolver.Type(SiteType.Morizon, _filter);
            var deal = FilterResolver.Deal(SiteType.Morizon, _filter);
            var market = FilterResolver.Market(SiteType.Morizon, _filter);

            var domain = "https://www.morizon.pl";

            if (_filter.Deal == DealType.Rent)
            {
                domain = $"{domain}/do-wynajecia";
            }

            var result =
                $"{domain}/{type}/{_filter.LocationRaw}/?";

            var divider = "ps%5B";

            if (_filter.PriceFrom != 0)
                result = result + $"{divider}price_from%5D={_filter.PriceFrom}&";

            if (_filter.PriceTo != 0)
                result = result + $"{divider}price_to%5D={_filter.PriceTo}&";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"{divider}living_area_from%5D={_filter.FlatAreaFrom}&";

            if (_filter.FlatAreaTo != 0)
                result = result + $"{divider}living_area_to%5D={_filter.FlatAreaTo}&";

            result = result + $"{divider}date_filter%5D=1&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}market_type%5D%5B0%5D={market}";

            return result;
        }
    }
}