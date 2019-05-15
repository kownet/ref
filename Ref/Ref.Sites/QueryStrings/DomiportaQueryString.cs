using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class DomiportaQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "mieszkanie";
            var deal = dealType == DealType.Sale ? "sprzedam" : "wynajme";
            var market = "Wtorny";

            var dist = district is null ? "" : $"%2C%20{district.Name}";

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            var result =
                $"https://www.domiporta.pl/{type}/{deal}?Localization={city.NameRaw}{dist}";

            result = result + $"&Rynek={market}";

            result = result + $"&SortingOrder=InsertionDate";

            return result;
        }

        public string Get(UserSubscriptionFilter userSubscriptionFilter)
        {
            var type = FilterResolver.Type(SiteType.DomiPorta, userSubscriptionFilter.Property);
            var deal = FilterResolver.Deal(SiteType.DomiPorta, userSubscriptionFilter.Deal);
            var market = FilterResolver.Market(SiteType.DomiPorta, userSubscriptionFilter.Market);

            var dist = userSubscriptionFilter.DistrictId is null ? "" : $"%2C%20{userSubscriptionFilter.District}";

            var cityName = userSubscriptionFilter.City;

            if (userSubscriptionFilter.City.Contains(' '))
                cityName = userSubscriptionFilter.City.Replace(' ', '-');

            var result =
                $"https://www.domiporta.pl/{type}/{deal}?Localization={cityName}{dist}";

            result = result + $"&Rynek={market}";

            result = result + $"&SortingOrder=InsertionDate";

            return result;
        }
    }
}