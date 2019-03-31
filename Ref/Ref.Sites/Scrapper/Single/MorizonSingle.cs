using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Scrapper.Single
{
    public class MorizonSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public MorizonSingle(IAppScrapperProvider appProvider)
            : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            var archive = doc.CssSelect(".exclamation").FirstOrDefault();

            if (!(archive is null))
            {
                result.IsDeleted = true;
            }
            else
            {
                var content = doc.CssSelect(".description").FirstOrDefault();

                if (!(content is null))
                {
                    if (!string.IsNullOrWhiteSpace(content.InnerText))
                    {
                        result.Content = content.InnerText.Trim();
                    }
                }
            }

            return result;
        }
    }
}