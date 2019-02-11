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
    public class Olx : SiteToScrapp, ISiteToScrapp
    {
        public Olx(
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

            var searchQuery = QueryStringProvider(SiteType.Olx).Get(filter);

            var doc = ScrapThis(searchQuery);

            var noResult = doc.CssSelect(".emptynew ").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Olx).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}page={i}");

                var listing = doc.CssSelect(".offer-wrapper");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var offer in listing)
                        {
                            var ad = new Ad
                            {
                                SiteType = SiteType.Olx
                            };

                            var table = offer.CssSelect("table").FirstOrDefault();

                            if(!(table is null))
                            {
                                ad.Id = table.ByAttribute("data-id");

                                var link = table.CssSelect(".linkWithHash").FirstOrDefault();

                                if(!(link is null))
                                {
                                    var url = link.ByAttribute("href");

                                    ad.Url = url;
                                    ad.Header = url.Replace("https://www.olx.pl/oferta/", string.Empty);
                                }

                                ad.Price = table.ByClass("price", @"[^0-9,.-]");
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