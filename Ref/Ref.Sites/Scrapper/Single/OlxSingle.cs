using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Scrapper.Single
{
    public class OlxSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public OlxSingle(IAppScrapperProvider appProvider)
            : base(appProvider)
        {
        }

        public SingleScrappResponse SingleScrapp(Offer offer)
        {
            var result = new SingleScrappResponse();

            var scrap = ScrapThis($@"{offer.Url}");

            if (!scrap.Succeed)
                return result;

            if (offer.Url.Contains("https://www.otodom.pl"))
                return new SingleScrappResponse { IsRedirected = true };

            var doc = scrap.HtmlNode;

            var content = doc.CssSelect("#textContent").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    result.Content = content.InnerText.Trim();
                }
            }

            var descContent = doc.CssSelect(".descriptioncontent").FirstOrDefault();

            if (!(descContent is null))
            {
                var items = descContent.CssSelect(".item");

                if (items.AnyAndNotNull())
                {
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.InnerText))
                        {
                            if (item.InnerText.Contains("Cena za m²"))
                            {
                                var val = item.ByClass("value", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    if (val.Contains("."))
                                    {
                                        var ppms = val.Split(".")[0];

                                        if (int.TryParse(ppms, out int ppm))
                                        {
                                            result.PricePerMeter = ppm;
                                        }
                                    }
                                }
                            }

                            if (item.InnerText.Contains("Poziom"))
                            {
                                var val = item.ByClass("value", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    if (int.TryParse(val, out int f))
                                    {
                                        result.Floor = f;
                                    }
                                }
                            }

                            if (item.InnerText.Contains("Powierzchnia"))
                            {
                                var val = item.ByClass("value", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    if (val.Contains(","))
                                    {
                                        var toParse = val.Split(",")[0];

                                        if (int.TryParse(toParse, out int a))
                                        {
                                            result.Area = a;
                                        }
                                    }
                                    else
                                    {
                                        if (int.TryParse(val, out int a))
                                        {
                                            result.Area = a;
                                        }
                                    }
                                }
                            }

                            if (item.InnerText.Contains("Liczba pokoi"))
                            {
                                var val = item.ByClass("value", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    if (int.TryParse(val, out int r))
                                    {
                                        result.Rooms = r;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(!result.PricePerMeter.HasValue || (result.PricePerMeter.HasValue && result.PricePerMeter.Value == 0))
            {
                if (result.Area.HasValue && offer.Price > 0)
                {
                    if (result.Area.Value > 0)
                    {
                        var ppm = offer.Price / result.Area.Value;

                        if (ppm > 0)
                        {
                            result.PricePerMeter = ppm;
                        }
                    }
                }
            }

            return result;
        }
    }
}