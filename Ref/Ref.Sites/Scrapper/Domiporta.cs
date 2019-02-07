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
    public class Domiporta : SiteToScrapp, ISiteToScrapp
    {
        public Domiporta(
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

            var searchQuery = QueryStringProvider(SiteType.DomiPorta).Get(filter);

            var doc = Scrapper.Load(searchQuery).DocumentNode;

            var noResult = doc.CssSelect(".alert__title ").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result
                };
            }

            int pages = PageProvider(SiteType.DomiPorta).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = Scrapper.Load($@"{searchQuery}&PageNumber={i}").DocumentNode;

                var listing = doc.CssSelect(".listing").FirstOrDefault();

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
                                    SiteType = SiteType.DomiPorta,
                                    Price = article.ByClass("sneakpeak__details_price", @"[^0-9,.-]"),
                                    Area = article.ByClass("sneakpeak__details_item--area", @"[^0-9,.-]"),
                                    PricePerMeter = article.ByClass("sneakpeak__details_item--price", @"[^0-9,.-]").RemoveLastIf("2"),
                                };

                                var idPin = article.CssSelect(".sneakpeak__pin").FirstOrDefault();

                                if (!(idPin is null))
                                {
                                    var idInput = idPin.CssSelect("input").FirstOrDefault();

                                    if (!(idInput is null))
                                    {
                                        ad.Id = idInput.ByAttribute("value");
                                    }
                                }

                                var cnt = article.CssSelect(".sneakpeak__picture_container").FirstOrDefault();

                                if (!(cnt is null))
                                {
                                    ad.Url = $"https://www.domiporta.pl{cnt.ByAttribute("href")}";
                                    ad.Header = cnt.ByAttribute("title");
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