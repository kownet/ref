using Ref.Shared.Providers;

namespace Ref.Sites.Helpers
{
    public static class QueryStringBuilder
    {
        public static string OtoDom(IFilterProvider filter)
        {
            var result =
                $"https://www.otodom.pl/{filter.Deal()}/{filter.Type()}/{filter.Location()}/?";

            var divider = "search%5B";

            if (filter.PriceFrom() != 0)
                result = result + $"{divider}filter_float_price%3Afrom%5D={filter.PriceFrom()}&";

            if (filter.PriceTo() != 0)
                result = result + $"{divider}filter_float_price%3Ato%5D={filter.PriceTo()}&";

            if (filter.FlatAreaFrom() != 0)
                result = result + $"{divider}filter_float_m%3Afrom%5D={filter.FlatAreaFrom()}&";

            if (filter.FlatAreaTo() != 0)
                result = result + $"{divider}filter_float_m%3Ato%5D={filter.FlatAreaTo()}&";

            result = result + $"{divider}created_since%5D={filter.Newest()}&";

            if (string.IsNullOrWhiteSpace(filter.Market()))
                result = result + $"{divider}filter_enum_market%5D%5B0%5D={filter.Market()}&";

            return result;
        }
    }
}