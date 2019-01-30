﻿using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class MorizonQueryString : IQueryString
    {
        private readonly IFilterProvider _filter;

        public MorizonQueryString(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Morizon, _filter);
            var deal = FilterResolver.Deal(SiteType.Morizon, _filter);
            var market = FilterResolver.Market(SiteType.Morizon, _filter);

            var result =
                $"https://www.morizon.pl/{type}/{_filter.Location()}/?";

            var divider = "ps%5B";

            if (_filter.PriceFrom() != 0)
                result = result + $"{divider}price_from%5D={_filter.PriceFrom()}&";

            if (_filter.PriceTo() != 0)
                result = result + $"{divider}price_to%5D={_filter.PriceTo()}&";

            if (_filter.FlatAreaFrom() != 0)
                result = result + $"{divider}living_area_from%5D={_filter.FlatAreaFrom()}&";

            if (_filter.FlatAreaTo() != 0)
                result = result + $"{divider}living_area_to%5D={_filter.FlatAreaTo()}&";

            result = result + $"{divider}date_filter%5D={_filter.Newest()}&";

            if (!string.IsNullOrWhiteSpace(market))
                result = result + $"{divider}market_type%5D%5B0%5D={market}";

            return result;
        }
    }
}