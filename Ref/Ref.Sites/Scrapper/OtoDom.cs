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
    public class OtoDom : SiteToScrapp, ISiteToScrapp
    {
        public OtoDom(
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

            var searchQuery = QueryStringProvider(SiteType.OtoDom).Get(filter);

            var doc = Scrapper.Load(searchQuery).DocumentNode;

            var noResult = doc.CssSelect(".search-location-extended-warning").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result
                };
            }

            int pages = PageProvider(SiteType.OtoDom).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = Scrapper.Load($@"{searchQuery}page={i}").DocumentNode;

                var listing = doc.CssSelect(".section-listing__row-content");

                if (!(listing is null))
                {
                    var articles = listing.CssSelect("article");

                    if (!(articles is null))
                    {
                        if (articles.AnyAndNotNull())
                        {
                            foreach (var article in articles)
                            {
                                var ad = new Ad
                                {
                                    Id = article.ByAttribute("data-tracking-id"),
                                    Url = article.ByAttribute("data-url"),
                                    Header = article.ByClass("offer-item-title"),
                                    Price = article.ByClass("offer-item-price", @"[^0-9,.-]"),
                                    Rooms = article.ByClass("offer-item-rooms", @"[^0-9 ,.-]"),
                                    Area = article.ByClass("offer-item-area", @"[^0-9 ,.-]"),
                                    PricePerMeter = article.ByClass("offer-item-price-per-m", @"[^0-9 ,.-]").RemoveLastIf("2"),
                                    SiteType = SiteType.OtoDom
                                };

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