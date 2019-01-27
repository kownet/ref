using Ref.Data.Models;
using Ref.Shared.Providers;

namespace Ref.Sites.Helpers.QueryStrings
{
    public class MorizonQs : IQs
    {
        private readonly IFilterProvider _filter;

        public MorizonQs(IFilterProvider filter)
        {
            _filter = filter;
        }

        public string Get()
        {
            var type = FilterResolver.Type(SiteType.Morizon, _filter);
            var deal = FilterResolver.Deal(SiteType.Morizon, _filter);
            var market = FilterResolver.Market(SiteType.Morizon, _filter);

            var result =
                $"";

            return result;
        }
    }
}