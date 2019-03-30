using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper.Single
{
    public class AdresowoSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public AdresowoSingle(IAppScrapperProvider appProvider) : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}", "iso-8859-2");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            return result;
        }
    }
}