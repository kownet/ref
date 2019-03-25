﻿using HtmlAgilityPack;
using Ref.Data.Models;
using Ref.Data.Repositories.Standalone;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using Ref.Sites.Pages;
using Ref.Sites.QueryStrings;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            var searchQuery = QueryStringProvider(SiteType.DomiPorta).Get(city, dealType);

            var scrap = ScrapThis(searchQuery);

            if (!scrap.Succeed)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ExceptionAccured = scrap.ExceptionAccured,
                    ExceptionMessage = scrap.ExceptionMessage
                };
            }

            HtmlNode doc = scrap.HtmlNode;

            var noResult = doc.CssSelect(".alert__title ").FirstOrDefault();

            if (noResult != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = false
                };
            }

            int pages = PageProvider(SiteType.DomiPorta).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.DomiPorta);
            result.Change(o => o.Deal = dealType);
            result.Change(o => o.CityId = city.Id);

            return new ScrappResponse
            {
                Offers = result
            };
        }

        public SiteResponse Search(SearchFilter filter)
        {
            var result = new List<Ad>();

            var searchQuery = QueryStringProvider(SiteType.DomiPorta).Get(filter);

            var scrap = ScrapThis(searchQuery);

            if (!scrap.Succeed)
            {
                return new SiteResponse
                {
                    Advertisements = new List<Ad>(),
                    ExceptionAccured = scrap.ExceptionAccured,
                    ExceptionMessage = scrap.ExceptionMessage
                };
            }

            HtmlNode doc = scrap.HtmlNode;

            var noResult = doc.CssSelect(".alert__title ").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.DomiPorta).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}&PageNumber={i}").HtmlNode;

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
                                    ad.Header = HttpUtility.HtmlDecode(cnt.ByAttribute("title"));
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

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc)
        {
            var result = new List<Offer>();

            /// order by desc, no need to grab all sites
            if (pages > AppProvider.Pages())
                pages = AppProvider.Pages();

            for (int i = 1; i <= pages; i++)
            {
                var scrap = ScrapThis($@"{searchQuery}&PageNumber={i}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

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
                                var ad = new Offer
                                {
                                    DateAdded = DateTime.Now
                                };

                                if (int.TryParse(article.ByClass("sneakpeak__details_price", @"[^0-9,.-]"), out int price))
                                {
                                    ad.Price = price;
                                }

                                var areaRaw = article.ByClass("sneakpeak__details_item--area", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(areaRaw))
                                {
                                    if(areaRaw.Length >= 2)
                                    {
                                        areaRaw = areaRaw.Replace(",", "").Substring(0, 2);

                                        if (int.TryParse(areaRaw, out int area))
                                        {
                                            ad.Area = area;
                                        }
                                    }
                                }

                                if (int.TryParse(article.ByClass("sneakpeak__details_item--price", @"[^0-9,.-]"), out int ppm))
                                {
                                    ad.PricePerMeter = ppm;
                                }

                                var idPin = article.CssSelect(".sneakpeak__pin").FirstOrDefault();

                                if (!(idPin is null))
                                {
                                    var idInput = idPin.CssSelect("input").FirstOrDefault();

                                    if (!(idInput is null))
                                    {
                                        ad.SiteOfferId = idInput.ByAttribute("value");
                                    }
                                }

                                var cnt = article.CssSelect(".sneakpeak__picture_container").FirstOrDefault();

                                if (!(cnt is null))
                                {
                                    ad.Url = $"https://www.domiporta.pl{cnt.ByAttribute("href")}";
                                    ad.Header = HttpUtility.HtmlDecode(cnt.ByAttribute("title"));
                                }

                                if (ad.IsValidToAdd())
                                    result.Add(ad);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}