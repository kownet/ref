﻿using Ref.Data.Models;
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
    public class Gratka : SiteToScrapp, ISiteToScrapp
    {
        public Gratka(
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

            var searchQuery = QueryStringProvider(SiteType.Gratka).Get(filter);

            var doc = ScrapThis(searchQuery);

            var noResult = doc.CssSelect(".content__emptyListInfo").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Gratka).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}&page={i}");

                var listing = doc.CssSelect(".content__listing");

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
                                    Id = article.ByAttribute("id"),
                                    Url = article.ByAttribute("data-href"),
                                    SiteType = SiteType.Gratka
                                };

                                ad.Header = article.ByClass("teaser__anchor");

                                var pricepm = article.ByClass("teaser__priceAdditional", @"[^0-9 ,.-]");

                                ad.PricePerMeter = pricepm.RemoveLastIf("2");

                                var price = article.CssSelect($".teaser__price").FirstOrDefault();

                                if(!(price is null))
                                {
                                    ad.Price = price.FirstChild != null ? price.FirstChild.InnerHtml.Trim().Replace(" ", string.Empty) : string.Empty;
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
                Advertisements = result,
                FilterDesc = filter.Description()
            };
        }
    }
}