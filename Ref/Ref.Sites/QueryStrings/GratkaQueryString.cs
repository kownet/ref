using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class GratkaQueryString : IQueryString
    {
        public string Get(City city, DealType dealType, District district = null)
        {
            var type = "mieszkania";
            var deal = dealType == DealType.Sale ? "sprzedaz" : "wynajem";
            var market = "wtorny";

            var dist = district is null ? "" : $"/{district.NameRaw}";

            var cityName = city.NameRaw;

            if (city.NameRaw.Contains(' '))
                cityName = city.NameRaw.Replace(' ', '-');

            var result =
                $"https://gratka.pl/nieruchomosci/{type}/{cityName}{dist}/{deal}?";

            result += $"&rynek={market}";

            result += $"&data-dodania-search=ostatnich-24h&sort=newest";

            return result;
        }

        public string Get(UserSubscriptionFilter userSubscriptionFilter)
        {
            var type = FilterResolver.Type(SiteType.Gratka, userSubscriptionFilter.Property);
            var deal = FilterResolver.Deal(SiteType.Gratka, userSubscriptionFilter.Deal);
            var market = FilterResolver.Market(SiteType.Gratka, userSubscriptionFilter.Market);

            var dist = userSubscriptionFilter.DistrictId is null ? "" : $"/{userSubscriptionFilter.District}";

            var cityName = userSubscriptionFilter.City;

            if (userSubscriptionFilter.City.Contains(' '))
                cityName = userSubscriptionFilter.City.Replace(' ', '-');

            var result =
                $"https://gratka.pl/nieruchomosci/{type}/{cityName}{dist}/{deal}?";

            result += $"&rynek={market}";

            result += $"&data-dodania-search=ostatnich-24h&sort=newest";

            return result;
        }
    }
}