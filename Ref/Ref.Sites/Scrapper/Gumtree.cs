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

namespace Ref.Sites.Scrapper
{
    public class Gumtree : SiteToScrapp, ISiteToScrapp
    {
        public Gumtree(
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

            var searchQuery = QueryStringProvider(SiteType.Gumtree).Get(filter);

            var code = FilterResolver.Code(filter);

            var doc = Scrapper.Load(searchQuery).DocumentNode;

            int.TryParse(doc.ByClass("count", @"[^0-9]"), out int count);

            if (count == 0)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Gumtree).Get(doc, code);

            for (int i = 1; i <= pages; i++)
            {
                var sq = searchQuery.Replace($"{code}1", $"page-{i}/{code}{i}");

                doc = Scrapper.Load($@"{sq}").DocumentNode;

                var listing = doc.CssSelect(".result-link");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var article in listing)
                        {
                            var ad = new Ad
                            {
                                SiteType = SiteType.Gumtree,
                                Price = article.ByClass("amount", @"[^0-9,.-]")
                            };

                            var link = article.CssSelect(".href-link").FirstOrDefault();

                            if (!(link is null))
                            {
                                ad.Url = $"https://www.gumtree.pl{link.ByAttribute("href")}";
                                ad.Header = link.InnerText;

                                if (!string.IsNullOrWhiteSpace(ad.Url))
                                {
                                    ad.Id = ad.Url.Split("/").Last();
                                }
                            }

                            if (ad.IsValid())
                                result.Add(ad);
                        }
                    }
                }
            }
            return new SiteResponse
            {
                FilterName = filter.Name,
                Advertisements = result,
                FilterDesc = filter.Description()
            };
        }
    }
}