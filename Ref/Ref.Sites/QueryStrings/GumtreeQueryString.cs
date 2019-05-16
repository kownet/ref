using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class GumtreeQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "s-mieszkania-i-domy";
            var deal = dealType == DealType.Sale ? "-sprzedam-i-kupie" : "-do-wynajecia";

            var code = dealType == DealType.Sale ? city.GtCodeSale : city.GtCodeRent;

            var houseOrFlat = dealType == DealType.Sale ? "mieszkanie/" : string.Empty;

            var page = 1;

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains('-'))
                cityName = city.NameRaw.Replace('-', '+');

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            if (!(district is null) && city.NameRaw == "warszawa") // gumtree wspiera tylko dzielnice warszawy
            {
                cityName = district.NameRaw;
                code = dealType == DealType.Sale ? district.GtCodeSale : district.GtCodeRent;
            }

            var result =
                $"https://www.gumtree.pl/{type}{deal}/{cityName}/{houseOrFlat}{code}{page}?";

            result = result + $"&sort=dt&order=desc";

            return result;
        }

        public string Get(UserSubscriptionFilter userSubscriptionFilter)
        {
            var type = FilterResolver.Type(SiteType.Gumtree, userSubscriptionFilter.Property);
            var deal = FilterResolver.Deal(SiteType.Gumtree, userSubscriptionFilter.Deal);
            var market = FilterResolver.Market(SiteType.Gumtree, userSubscriptionFilter.Market);

            var code = userSubscriptionFilter.Deal == DealType.Sale ? userSubscriptionFilter.GumtreeCitySale : "";

            var houseOrFlat = userSubscriptionFilter.Deal == DealType.Sale
                ? (userSubscriptionFilter.Property == PropertyType.Flat ? "mieszkanie/" : "dom/")
                : string.Empty;

            var page = 1;

            var cityName = userSubscriptionFilter.City;

            if (userSubscriptionFilter.City.Contains('-'))
                cityName = userSubscriptionFilter.City.Replace('-', '+');

            if (userSubscriptionFilter.City.Contains(' '))
                cityName = userSubscriptionFilter.City.Replace(' ', '-');

            if (!(userSubscriptionFilter.DistrictId is null) && userSubscriptionFilter.City == "warszawa") // gumtree wspiera tylko dzielnice warszawy
            {
                cityName = userSubscriptionFilter.City;
                code = userSubscriptionFilter.Deal == DealType.Sale ? userSubscriptionFilter.GumtreeDistrictSale : "";
            }

            var result =
                $"https://www.gumtree.pl/{type}{deal}/{cityName}/{houseOrFlat}{code}{page}?";

            result = result + $"&sort=dt&order=desc";

            return result;
        }
    }
}