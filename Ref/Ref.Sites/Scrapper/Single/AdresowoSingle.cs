using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Scrapper.Single
{
    public class AdresowoSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public AdresowoSingle(IAppScrapperProvider appProvider)
            : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}", "iso-8859-2");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            var content = doc.CssSelect("#offer-price").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    if(content.InnerText.Contains("Usunięte"))
                    {
                        result.IsDeleted = true;
                    }
                    else
                    {
                        var rawContent = doc.CssSelect("#offer-description").FirstOrDefault();

                        if (!(rawContent is null))
                        {
                            if (!string.IsNullOrWhiteSpace(rawContent.InnerText))
                            {
                                result.Content = rawContent.InnerText.Trim();
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}