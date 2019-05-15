using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
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
            string result = "";
            return result;
        }

        public string Get(SearchFilter _filter)
        {
            var type = FilterResolver.Type(SiteType.DomiPorta, _filter.Property);
            var deal = FilterResolver.Deal(SiteType.DomiPorta, _filter.Deal);
            var market = FilterResolver.Market(SiteType.DomiPorta, _filter.Market);

            var result =
                $"https://www.domiporta.pl/{type}/{deal}?Localization={_filter.Location}";

            if (_filter.PriceFrom != 0)
                result = result + $"&Price.From={_filter.PriceFrom}";

            if (_filter.PriceTo != 0)
                result = result + $"&Price.To={_filter.PriceTo}";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"&Surface.From={_filter.FlatAreaFrom}";

            if (_filter.FlatAreaTo != 0)
                result = result + $"&Surface.To={_filter.FlatAreaTo}";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"&Rynek={market}";

            result = result + $"&Rodzaj=Bezposrednie&SortingOrder=InsertionDate";

            return result;
        }
    }
}