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
    public class Adresowo : SiteToScrapp, ISiteToScrapp
    {
        public Adresowo(
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

            var searchQuery = QueryStringProvider(SiteType.Adresowo).Get(filter);

            var newest = filter.Newest == 1 ? "od" : string.Empty;

            var doc = ScrapThis(searchQuery, "iso-8859-2");

            if (doc.InnerHtml.Contains("jest pusta"))
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Adresowo).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($"{searchQuery}{i}{newest}", "iso-8859-2");

                var listing = doc.CssSelect(".offer-list");

                if (!(listing is null))
                {
                    var articles = listing.CssSelect("tr");

                    if (!(articles is null))
                    {
                        if (articles.AnyAndNotNull())
                        {
                            foreach (var article in articles)
                            {
                                var ad = new Ad
                                {
                                    SiteType = SiteType.Adresowo
                                };

                                var id = article.ByAttribute("id");

                                ad.Id = !string.IsNullOrWhiteSpace(id) ? id.Remove(0, 1) : string.Empty;
                                ad.Url = $"https://adresowo.pl/o/{ad.Id}";

                                ad.Header = article.ByClass("address");
                                ad.Price = article.ByClass("price", @"[^0-9,.-]");
                                ad.PricePerMeter = article.ByClass("price-per-unit", @"[^0-9 ,.-]").RemoveLastIf("2");

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
                Advertisements = result,
                FilterDesc = filter.Description()
            };
        }
    }
}