using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper.Single
{
    public class DomiportaSingle : SingleSiteToScrapp, ISingleSiteToScrapp
    {
        public DomiportaSingle(IAppScrapperProvider appProvider)
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

            var archive = doc.CssSelect(".archive").FirstOrDefault();

            if (!(archive is null))
            {
                result.IsDeleted = true;
            }
            else
            {
                var content = doc.CssSelect(".description__container").FirstOrDefault();

                if (!(content is null))
                {
                    if (!string.IsNullOrWhiteSpace(content.InnerText))
                    {
                        result.Content = content.InnerText.Trim();
                    }
                }

                var elements = doc.CssSelect(".icons-list");

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

                if (!result.Floor.HasValue)
                {
                    var elementsList = doc.CssSelect(".features__list");

                    if (elementsList.AnyAndNotNull())
                    {
                        foreach (var element in elementsList)
                        {
                            if (!string.IsNullOrWhiteSpace(element.InnerText.Trim()))
                            {
                                var names = element.CssSelect(".features__item_name");

                                var counter = 0;

                                if(names.AnyAndNotNull())
                                {
                                    foreach (var name in names)
                                    {
                                        if (!(name is null))
                                        {
                                            if (name.InnerText.Contains("Piętro"))
                                            {
                                                var val = element.CssSelect(".features__item_value").Skip(counter).FirstOrDefault();

                                                if (!(val is null))
                                                {
                                                    if(!string.IsNullOrWhiteSpace(val.InnerText))
                                                    {
                                                        if(val.InnerText.Contains("Parter"))
                                                        {
                                                            result.Floor = 0;
                                                        }
                                                        else
                                                        {
                                                            if (int.TryParse(Regex.Replace(val.InnerText.Trim(), regex, string.Empty).Trim(), out int a))
                                                            {
                                                                result.Floor = a;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        counter++;
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