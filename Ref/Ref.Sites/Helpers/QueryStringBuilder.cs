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
                    return new OtoDomQueryString(_filterProvider).Get();
                case SiteType.Olx:
                    return new OlxQueryString(_filterProvider).Get(); ;
                case SiteType.Adresowo:
                    return new AdresowoQueryString(_filterProvider).Get();
                case SiteType.DomiPorta:
                    return new DomiportaQueryString(_filterProvider).Get();
                case SiteType.Gratka:
                    return new GratkaQueryString(_filterProvider).Get();
                case SiteType.Morizon:
                    return new MorizonQueryString(_filterProvider).Get();
                case SiteType.Gumtree:
                    return new GumtreeQueryString(_filterProvider).Get();
                default: return string.Empty;
            }
        }
    }
}