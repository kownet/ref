using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper.Single
{
    public class GratkaSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public GratkaSingle(IAppScrapperProvider appProvider)
            : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            string regex = @"[^0-9,.-]";

            var doc = scrap.HtmlNode;

            var archive = doc.CssSelect(".errorPage__title").FirstOrDefault();

            if (!(archive is null))
            {
                result.IsDeleted = true;
            }

            var content = doc.CssSelect(".description__container").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    result.Content = content.InnerText.Trim();
                }
            }

            var floorOverview = doc.CssSelect(".parameters__rolled").FirstOrDefault();

            if (!(floorOverview is null))
            {
                var elements = floorOverview.CssSelect("li");

                if (elements.AnyAndNotNull())
                {
                    foreach (var element in elements)
                    {
                        if (!string.IsNullOrWhiteSpace(element.InnerText.Trim()))
                        {
                            if (element.InnerText.Contains("Piętro"))
                            {
                                if (int.TryParse(Regex.Replace(element.InnerText.Trim(), regex, string.Empty).Trim(), out int a))
                                {
                                    result.Floor = a;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}