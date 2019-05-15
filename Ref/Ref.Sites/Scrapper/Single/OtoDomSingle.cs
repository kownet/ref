using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper.Single
{
    public class OtoDomSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public OtoDomSingle(IAppScrapperProvider appProvider)
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

            var content = doc.CssSelect(".section-description").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText.Trim()))
                {
                    result.Content = content.InnerText.Replace("Opis", "");
                }
            }

            var floorOverview = doc.CssSelect(".section-overview").FirstOrDefault();

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

            if(offer.PricePerMeter == 0)
            {
                if (offer.Area > 0 && offer.Price > 0)
                {
                    var ppm = offer.Price / result.Area;

                    if (ppm > 0)
                    {
                        result.PricePerMeter = ppm;
                    }
                }
            }

            return result;
        }
    }
}