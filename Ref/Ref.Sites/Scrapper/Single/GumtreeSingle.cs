using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper.Single
{
    public class GumtreeSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public GumtreeSingle(IAppScrapperProvider appProvider)
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

            var content = doc.CssSelect(".description").FirstOrDefault();

            if (!(content is null))
            {
                if (!string.IsNullOrWhiteSpace(content.InnerText))
                {
                    result.Content = content.InnerText.Trim();
                }
            }

            var details = doc.CssSelect(".selMenu").FirstOrDefault();

            if (!(details is null))
            {
                var lis = details.CssSelect("li");

                if (lis.AnyAndNotNull())
                {
                    foreach (var li in lis)
                    {
                        if (!string.IsNullOrWhiteSpace(li.InnerText))
                        {
                            var name = li.CssSelect(".name").FirstOrDefault();
                            var val = li.CssSelect(".value").FirstOrDefault();

                            if (!(name is null) && !(val is null))
                            {
                                if (!string.IsNullOrWhiteSpace(name.InnerText))
                                {
                                    if (name.InnerText.Contains("Wielkość (m2)"))
                                    {
                                        var vali = val.InnerText;

                                        if (!string.IsNullOrWhiteSpace(vali))
                                        {
                                            if (int.TryParse(Regex.Replace(vali, regex, string.Empty).Trim(), out int a))
                                            {
                                                result.Area = a;
                                            }
                                        }
                                    }

                                    if (name.InnerText.Contains("Liczba pokoi"))
                                    {
                                        var vali = val.InnerText;

                                        if (!string.IsNullOrWhiteSpace(vali))
                                        {
                                            if (vali.ToLower().Contains("kawalerka") || vali.ToLower().Contains("garsoniera"))
                                            {
                                                result.Rooms = 1;
                                            }
                                            if (int.TryParse(Regex.Replace(vali, regex, string.Empty).Trim(), out int r))
                                            {
                                                result.Rooms = r;
                                            }
                                        }
                                    }

                                    if (name.InnerText.Contains("Na sprzedaż przez"))
                                    {
                                        var vali = val.InnerText;

                                        if (!string.IsNullOrWhiteSpace(vali))
                                        {
                                            if (string.Equals("Agencja", vali))
                                            {
                                                result.IsFromAgency = true;
                                                result.IsFromPrivate = false;
                                            }

                                            if (string.Equals("Właściciel", vali))
                                            {
                                                result.IsFromAgency = false;
                                                result.IsFromPrivate = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

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

            return result;
        }
    }
}