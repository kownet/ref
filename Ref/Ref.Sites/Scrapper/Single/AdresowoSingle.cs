using Ref.Data.Models;
using Ref.Shared.Extensions;
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
            var result = new SingleScrappResponse
            {
                Area = offer.Area,
                PricePerMeter = offer.PricePerMeter
            };

            var scrap = ScrapThis($@"{offer.Url}", "iso-8859-2");

            if (!scrap.Succeed)
                return result;

            var doc = scrap.HtmlNode;

            var content = doc.CssSelect("#offer-price").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    if (content.InnerText.Contains("Usunięte"))
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

                        var summary = doc.CssSelect("#offer-summary").FirstOrDefault();

                        if (!(summary is null))
                        {
                            var ul = summary.CssSelect("li");

                            if (ul.AnyAndNotNull())
                            {
                                foreach (var li in ul)
                                {
                                    if (!string.IsNullOrWhiteSpace(li.InnerText))
                                    {
                                        if (li.InnerText.Contains("-pokojowe"))
                                        {
                                            int index = li.InnerText.IndexOf('-');

                                            var ir = index - 1;

                                            if (ir >= 0)
                                            {
                                                var rooms = li.InnerText.ElementAt(ir).ToString();

                                                if (int.TryParse(rooms, out int r))
                                                {
                                                    result.Rooms = r;
                                                }
                                            }
                                        }

                                        if (li.InnerText.Contains("&nbsp;piętro"))
                                        {
                                            int index = li.InnerText.IndexOf("&nbsp;piętro");

                                            var ir = index - 1;

                                            if (ir >= 0)
                                            {
                                                var floor = li.InnerText.ElementAt(ir).ToString();

                                                if (int.TryParse(floor, out int f))
                                                {
                                                    result.Floor = f;
                                                }
                                            }
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