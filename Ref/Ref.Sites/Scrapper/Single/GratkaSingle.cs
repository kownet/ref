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
            var result = new SingleScrappResponse
            {
                Area = offer.Area,
                PricePerMeter = offer.PricePerMeter
            };

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

            result.IsFromAgency = false;
            result.IsFromPrivate = true;

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

                            if (element.InnerText.Contains("Numer referencyjny"))
                            {
                                result.IsFromAgency = true;
                                result.IsFromPrivate = false;
                            }
                        }
                    }
                }
            }

            if(offer.Area == 0)
            {
                if(offer.Price != 0 && offer.PricePerMeter != 0)
                {
                    var area = offer.Price / offer.PricePerMeter;

                    if (area > 0)
                    {
                        result.Area = area;
                    }
                }
            }

            return result;
        }
    }
}