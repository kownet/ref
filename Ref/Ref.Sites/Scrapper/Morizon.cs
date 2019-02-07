using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper
{
    public class Morizon : SiteToScrapp, ISiteToScrapp
    {
        public Morizon(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = QueryStringProvider(SiteType.Morizon).Get(filter);

            var doc = Scrapper.Load(searchQuery).DocumentNode;

            var noResult = doc.CssSelect(".message-title").FirstOrDefault();

            string regex = @"[^0-9 ,.-]";

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result
                };
            }

            int pages = PageProvider(SiteType.Morizon).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = Scrapper.Load($@"{searchQuery}&page={i}").DocumentNode;

                var articles = doc.CssSelect(".row--property-list");

                if (!(articles is null))
                {
                    if (articles.AnyAndNotNull())
                    {
                        foreach (var article in articles)
                        {
                            if (!string.IsNullOrWhiteSpace(article.ByAttribute("data-id")))
                            {
                                var ad = new Ad
                                {
                                    Id = article.ByAttribute("data-id"),
                                    Header = article.ByClass("single-result__title"),
                                    Price = article.ByClass("single-result__price", @"[^0-9 ,.-]"),
                                    PricePerMeter = article.ByClass("single-result__price--currency", @"[^0-9 ,.-]"),
                                    SiteType = SiteType.Morizon
                                };

                                var url = article.CssSelect(".property_link").FirstOrDefault();

                                if (!(url is null))
                                {
                                    ad.Url = url.ByAttribute("href");
                                }

                                var info = article.CssSelect(".info-description").FirstOrDefault();

                                if (!(info is null))
                                {
                                    var elements = info.CssSelect("li");

                                    if (!(elements is null))
                                    {
                                        if (elements.AnyAndNotNull())
                                        {
                                            ad.Rooms = Regex.Replace(elements.First().InnerText, regex, string.Empty).Trim();
                                            ad.Area = Regex.Replace(elements.SecondLast().InnerText, regex, string.Empty).Trim();
                                        }
                                    }
                                }

                                if (ad.IsValid())
                                    result.Add(ad);
                            }
                        }
                    }
                }
            }

            return new SiteResponse
            {
                FilterName = filter.Name,
                Advertisements = result
            };
        }
    }
}