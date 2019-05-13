using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

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
            var result = new SingleScrappResponse
            {
                Area = offer.Area,
                PricePerMeter = offer.PricePerMeter
            };

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            string regex = @"[^0-9,.-]";

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

            var pars = doc.CssSelect(".propertyParams").FirstOrDefault();

            if (!(pars is null))
            {
                var trs = pars.CssSelect("tr");

                if (trs.AnyAndNotNull())
                {
                    foreach (var tr in trs)
                    {
                        if (!string.IsNullOrWhiteSpace(tr.InnerText))
                        {
                            if (tr.InnerText.Contains("Piętro"))
                            {
                                if (tr.InnerText.Contains("/"))
                                {
                                    var splitted = tr.InnerText.Split("/")[0];

                                    if (!string.IsNullOrWhiteSpace(splitted))
                                    {
                                        if (int.TryParse(Regex.Replace(splitted, regex, string.Empty).Trim(), out int a))
                                        {
                                            result.Floor = a;
                                        }
                                    }
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