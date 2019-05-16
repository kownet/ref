using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class MorizonQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "mieszkania";
            var deal = dealType == DealType.Sale ? "" : "do-wynajecia";
            var market = "2";
            var newest = 1;

            var dist = district is null ? "" : $"/{district.NameRaw}";

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            var domain = "https://www.morizon.pl";

            if (dealType == DealType.Rent)
            {
                domain = $"{domain}/do-wynajecia";
            }

            var result =
                $"{domain}/{type}/{cityName}{dist}/?";

            var divider = "ps%5B";

            result = result + $"{divider}date_filter%5D={newest}&";

            result = result + $"{divider}market_type%5D%5B0%5D={market}";

            return result;
        }

        public string Get(UserSubscriptionFilter userSubscriptionFilter)
        {
            var type = FilterResolver.Type(SiteType.Morizon, userSubscriptionFilter.Property);
            var deal = FilterResolver.Deal(SiteType.Morizon, userSubscriptionFilter.Deal);
            var market = FilterResolver.Market(SiteType.Morizon, userSubscriptionFilter.Market);

            var newest = 1;

            var dist = userSubscriptionFilter.DistrictId is null ? "" : $"/{userSubscriptionFilter.District}";

            var cityName = userSubscriptionFilter.City;

            if (userSubscriptionFilter.City.Contains(' '))
                cityName = userSubscriptionFilter.City.Replace(' ', '-');

            var domain = "https://www.morizon.pl";

            if (userSubscriptionFilter.Deal == DealType.Rent)
            {
                domain = $"{domain}/do-wynajecia";
            }

            var result =
                $"{domain}/{type}/{cityName}{dist}/?";

            var divider = "ps%5B";

            result = result + $"{divider}date_filter%5D={newest}&";

            result = result + $"{divider}market_type%5D%5B0%5D={market}";

            return result;
        }
    }
}