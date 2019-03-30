using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;

namespace Ref.Sites.Scrapper.Single
{
    public class GumtreeSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public GumtreeSingle(IAppScrapperProvider appProvider) : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            return result;
        }
    }
}