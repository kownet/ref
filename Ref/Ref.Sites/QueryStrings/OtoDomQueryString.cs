using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
using Ref.Shared.Extensions;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class OtoDomQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "mieszkanie";
            var deal = dealType == DealType.Sale ? "sprzedaz" : "wynajem";
            var newest = 1; // from last 24hours
            var market = "secondary";

            var dist = district is null ? "?" : $"{district.NameRaw}/?";

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            var result =
                $"https://www.otodom.pl/{deal}/{type}/{cityName}/{dist}";

            var divider = "search%5B";

            result = result + $"{divider}created_since%5D={newest}&";

            if (dealType == DealType.Sale)
            {
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";
            }

            return result.RemoveLastIf("&");
        }

        public string Get(UserSubscriptionFilter userSubscriptionFilter)
        {
            var type = FilterResolver.Type(SiteType.OtoDom, userSubscriptionFilter.Property);
            var deal = FilterResolver.Deal(SiteType.OtoDom, userSubscriptionFilter.Deal);
            var market = FilterResolver.Market(SiteType.OtoDom, userSubscriptionFilter.Market);
            var newest = 1; // from last 24hours

            type = userSubscriptionFilter.Market == MarketType.Secondary
                ? $"{type}"
                : $"nowe-{type}";

            var dist = userSubscriptionFilter.DistrictId is null ? "?" : $"{userSubscriptionFilter.District}/?";

            var cityName = userSubscriptionFilter.City;

            if (userSubscriptionFilter.City.Contains(' '))
                cityName = userSubscriptionFilter.City.Replace(' ', '-');

            var result =
                $"https://www.otodom.pl/{deal}/{type}/{cityName}/{dist}";

            var divider = "search%5B";

            result = result + $"{divider}created_since%5D={newest}&";

            if (userSubscriptionFilter.Deal == DealType.Sale)
            {
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";
            }

            return result.RemoveLastIf("&");
        }

        public string Get(SearchFilter _filter)
        {
            var type = FilterResolver.Type(SiteType.OtoDom, _filter.Property);
            var deal = FilterResolver.Deal(SiteType.OtoDom, _filter.Deal);
            var market = FilterResolver.Market(SiteType.OtoDom, _filter.Market);

            type = _filter.Market == MarketType.Secondary
                ? $"{type}"
                : $"nowe-{type}";

            var result =
                $"https://www.otodom.pl/{deal}/{type}/{_filter.LocationRaw}/?";

            var divider = "search%5B";

            if (_filter.PriceFrom != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={_filter.PriceFrom}&";

            if (_filter.PriceTo != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={_filter.PriceTo}&";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={_filter.FlatAreaFrom}&";

            if (_filter.FlatAreaTo != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={_filter.FlatAreaTo}&";

            result = result + $"{divider}created_since%5D=1&";

            if (_filter.Deal == DealType.Sale)
            {
                if (!string.IsNullOrWhiteSpace(market))
                    result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";
            }

            return result.RemoveLastIf("&");
        }
    }
}