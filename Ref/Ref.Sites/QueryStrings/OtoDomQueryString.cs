using Ref.Data.Components;
using Ref.Data.Models;
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
    }
}