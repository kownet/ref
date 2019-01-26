using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers.QueryStrings;

namespace Ref.Sites.Helpers
{
    public class QueryStringBuilder
    {
        private readonly SiteType _siteType;
        private readonly IFilterProvider _filterProvider;

        public QueryStringBuilder(
            SiteType siteType,
            IFilterProvider filterProvider)
        {
            _siteType = siteType;
            _filterProvider = filterProvider;
        }

        public string Get()
        {
            switch (_siteType)
            {
                case SiteType.OtoDom:
                    return new OtoDomQs().Get(_filterProvider);
                case SiteType.Olx:
                    return new OlxQs().Get(_filterProvider); ;
                case SiteType.Adresowo:
                    return string.Empty;
                case SiteType.DomiPorta:
                    return string.Empty;
                case SiteType.Gratka:
                    return string.Empty;
                case SiteType.Morizon:
                    return string.Empty;
                case SiteType.Gumtree:
                    return string.Empty;
                default: return string.Empty;
            }
        }
    }
}