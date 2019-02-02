using Ref.Data.Models;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class OtoDomQueryString : IQueryString
    {
        private readonly Filter _filter;

        public OtoDomQueryString(Filter filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.OtoDom, _filter);
            var deal = FilterResolver.Deal(SiteType.OtoDom, _filter);
            var market = FilterResolver.Market(SiteType.OtoDom, _filter);

            var result =
                $"https://www.otodom.pl/{deal}/{type}/{_filter.Location}/?";

            var divider = "search%5B";

            if (_filter.PriceFrom != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={_filter.PriceFrom}&";

            if (_filter.PriceTo != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={_filter.PriceTo}&";

            if (_filter.FlatAreaFrom != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={_filter.FlatAreaFrom}&";

            if (_filter.FlatAreaTo != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={_filter.FlatAreaTo}&";

            result = result + $"{divider}created_since%5D={_filter.Newest}&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={market}&";

            return result;
        }
    }
}