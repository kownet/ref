using Ref.Data.Models;
using Ref.Sites.Helpers;
using System;

namespace Ref.Sites.QueryStrings
{
    public class DomiportaQueryString : IQueryString
    {
        public string Get(City city, DealType dealType)
        {
            throw new NotImplementedException();
        }

        public string Get(Filter _filter)
        {
            var type = FilterResolver.Type(SiteType.DomiPorta, _filter);
            var deal = FilterResolver.Deal(SiteType.DomiPorta, _filter);
            var market = FilterResolver.Market(SiteType.DomiPorta, _filter);

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

            if (_filter.Newest == 1)
                result = result + $"&Rodzaj=Bezposrednie&SortingOrder=InsertionDate";

            return result;
        }
    }
}