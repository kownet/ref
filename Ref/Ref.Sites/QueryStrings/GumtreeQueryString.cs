using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
using Ref.Sites.Helpers;

namespace Ref.Sites.QueryStrings
{
    public class GumtreeQueryString : IQueryString
    {
        public string Get(City city, DealType dealType)
        {
            var type = "s-mieszkania-i-domy";
            var deal = dealType == DealType.Sale ? "-sprzedam-i-kupie" : "-do-wynajecia";

            var code = dealType == DealType.Sale ? city.GtCodeSale : city.GtCodeRent;

            var houseOrFlat = dealType == DealType.Sale ? "mieszkanie/" : string.Empty;

            var page = 1;

            var result =
                $"https://www.gumtree.pl/{type}{deal}/{city.NameRaw}/{houseOrFlat}{code}{page}?";

            result = result + $"&sort=dt&order=desc";

            return result;
        }

        public string Get(SearchFilter _filter)
        {
            var type = FilterResolver.Type(SiteType.Gumtree, _filter);
            var deal = FilterResolver.Deal(SiteType.Gumtree, _filter);
            var market = FilterResolver.Market(SiteType.Gumtree, _filter);

            var code = FilterResolver.Code(_filter);

            var houseOrFlat = _filter.Deal == DealType.Sale
                ? (_filter.Property == PropertyType.Flat ? "mieszkanie/" : "dom/")
                : string.Empty;

            var page = 1;

            var result =
                $"https://www.gumtree.pl/{type}{deal}/{_filter.LocationRaw}/{houseOrFlat}{code}{page}?";

            var pFrom = _filter.PriceFrom == 0 ? string.Empty : _filter.PriceFrom.ToString();
            var pTo = _filter.PriceTo == 0 ? string.Empty : _filter.PriceTo.ToString();

            result = result + $"pr={pFrom},{pTo}";

            result = result + $"&df=ownr&sort=dt&order=desc";

            return result;
        }
    }
}