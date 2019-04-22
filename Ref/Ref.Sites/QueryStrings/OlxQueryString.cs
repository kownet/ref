using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
using Ref.Shared.Extensions;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class OlxQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "mieszkania";
            var deal = dealType == DealType.Sale ? "sprzedaz" : "wynajem";
            var market = "secondary";

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            var result =
                $"https://www.olx.pl/nieruchomosci/{type}/{deal}/{cityName}/?";

            var divider = "search%5B";

            result = result + $"{divider}Border%5D=created_at%3Adesc&";

            if (dealType == DealType.Sale)
            {
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";
            }

            if(!(district is null) && district.IsOlxAvailable)
            {
                result = result + $"{divider}district_id%5D={district.OtoDomId.Value}&";
            }

            return result.RemoveLastIf("&");
        }

        public string Get(SearchFilter _filter)
        {
            var type = FilterResolver.Type(SiteType.Olx, _filter);
            var deal = FilterResolver.Deal(SiteType.Olx, _filter);
            var market = FilterResolver.Market(SiteType.Olx, _filter);

            var result =
                $"https://www.olx.pl/nieruchomosci/{type}/{deal}/{_filter.LocationRaw}/?";

            var divider = "search%5B";

            if (_filter.PriceFrom != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={_filter.PriceFrom}&";

            if (_filter.PriceTo != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={_filter.PriceTo}&";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={_filter.FlatAreaFrom}&";

            if (_filter.FlatAreaTo != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={_filter.FlatAreaTo}&";

            if (_filter.Deal == DealType.Sale)
            {
                if (!string.IsNullOrWhiteSpace(market))
                    result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";
            }

            result = result + $"{divider}Border%5D=created_at%3Adesc&";

            result = result + $"{divider}private_business%5D=private&";

            return result.RemoveLastIf("&");
        }
    }
}