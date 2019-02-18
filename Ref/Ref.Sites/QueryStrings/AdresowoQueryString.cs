using Ref.Data.Models;
using Ref.Sites.Helpers;
using System;

namespace Ref.Sites.QueryStrings
{
    public class AdresowoQueryString : IQueryString
    {
        public string Get(City city, DealType dealType)
        {
            throw new NotImplementedException();
        }

        public string Get(Filter _filter)
        {
            var type = FilterResolver.Type(SiteType.Adresowo, _filter);
            var deal = FilterResolver.Deal(SiteType.Adresowo, _filter);
            var market = FilterResolver.Market(SiteType.Adresowo, _filter);

            var result =
                $"https://adresowo.pl/{type}/{_filter.LocationRaw}/";

            if (_filter.PriceFrom != 0)
                result = result + $"p{FormatPrice(_filter.PriceFrom)}";

            if (_filter.PriceTo != 0)
                result = result + $"-{FormatPrice(_filter.PriceTo)}_";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"a{_filter.FlatAreaFrom}";

            if (_filter.FlatAreaTo != 0)
                result = result + $"-{_filter.FlatAreaTo}";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"_{market}";

            if (_filter.Newest == 1)
                result = result + $"_l";

            return result;
        }

        private static int FormatPrice(int price)
        {
            if (price <= 10000)
                return 1;
            else if (price > 10000 && price < 100000)
            {
                int i = Math.Abs(price);
                while (i >= 10)
                    i /= 10;

                return i;
            }
            else if (price >= 100000 && price < 1000000)
                return price / 10000;
            else if (price >= 1000000)
                return price / 10000;
            else return 0;
        }
    }
}