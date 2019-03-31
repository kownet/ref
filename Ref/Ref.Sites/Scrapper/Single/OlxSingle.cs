using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Scrapper.Single
{
    public class OlxSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public OlxSingle(IAppScrapperProvider appProvider) : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            var content = doc.CssSelect("#textContent").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    result.Content = content.InnerText.Trim();
                }
            }

            return result;
        }
    }
}