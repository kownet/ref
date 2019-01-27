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
                    return new OtoDomQs(_filterProvider).Get();
                case SiteType.Olx:
                    return new OlxQs(_filterProvider).Get(); ;
                case SiteType.Adresowo:
                    return new AdresowoQs(_filterProvider).Get();
                case SiteType.DomiPorta:
                    return new DomiportaQs(_filterProvider).Get();
                case SiteType.Gratka:
                    return new GratkaQs(_filterProvider).Get();
                case SiteType.Morizon:
                    return new MorizonQs(_filterProvider).Get();
                case SiteType.Gumtree:
                    return new GumtreeQs(_filterProvider).Get();
                default: return string.Empty;
            }
        }
    }
}