﻿using HtmlAgilityPack;
using Ref.Data.Components;
using Ref.Data.Models;
using Ref.Data.Repositories;
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
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc)
        {
            var result = new List<Offer>();

            /// order by desc, no need to grab all sites
            if (pages > AppProvider.Pages())
                pages = AppProvider.Pages();

            for (int i = 1; i <= pages; i++)
            {
                var scrap = ScrapThis($"{searchQuery}{i}od", "iso-8859-2");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

                var listing = doc.CssSelect(".search-block");

                if (!(listing is null))
                {
                    var articles = listing.CssSelect(".search-results__item");

                    if (!(articles is null))
                    {
                        if (articles.AnyAndNotNull())
                        {
                            foreach (var article in articles)
                            {
                                var ad = new Offer
                                {
                                    Site = SiteType.Adresowo,
                                    DateAdded = DateTime.Now
                                };

                                ad.SiteOfferId = article.ByAttribute("data-public-code");
                                ad.Url = $"https://adresowo.pl/o/{ad.SiteOfferId}";

                                var header = article.CssSelect(".result-info__header").FirstOrDefault();

                                if (!(header is null))
                                {
                                    if (!string.IsNullOrWhiteSpace(header.InnerText))
                                    {
                                        ad.Header = header.InnerText
                                            .Trim()
                                            .Replace("\n", " ")
                                            .Replace("Mieszkanie na sprzedaż", "");
                                    }
                                }

                                var priceRaw = article.ByClass("result-info__price--total", @"[^0-9 ,.-]");

                                if (!string.IsNullOrWhiteSpace(priceRaw))
                                {
                                    priceRaw = priceRaw.Trim().Replace(" ", "");

                                    if (int.TryParse(priceRaw, out int price))
                                    {
                                        ad.Price = price;
                                    }
                                }

                                var ppmRaw = article.ByClass("result-info__price--per-sqm", @"[^0-9 ,.-]");

                                if (!string.IsNullOrWhiteSpace(priceRaw))
                                {
                                    ppmRaw = ppmRaw.RemoveLastIf("2");

                                    if (int.TryParse(ppmRaw, out int ppm))
                                    {
                                        ad.PricePerMeter = ppm;
                                    }
                                }

                                var area = article.CssSelect(".result-info__basic").Skip(1).FirstOrDefault();

                                if (!(area is null))
                                {
                                    if (!string.IsNullOrWhiteSpace(area.InnerText))
                                    {
                                        var areaRaw = area.InnerText
                                            .Replace("m&sup2;", "")
                                            .Trim();

                                        if (int.TryParse(areaRaw, out int areaa))
                                        {
                                            ad.Area = areaa;
                                        }
                                    }
                                }

                                if (ad.Area == 0)
                                {
                                    area = article.CssSelect(".result-info__basic").FirstOrDefault();

                                    if (!(area is null))
                                    {
                                        if (!string.IsNullOrWhiteSpace(area.InnerText))
                                        {
                                            var areaRaw = area.InnerText
                                                .Replace("m&sup2;", "")
                                                .Trim();

                                            if (int.TryParse(areaRaw, out int areaa))
                                            {
                                                ad.Area = areaa;
                                            }
                                        }
                                    }
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

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            if (userSubscriptionFilter.Deal == DealType.Rent)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            var searchQuery = QueryStringProvider(SiteType.Adresowo).Get(userSubscriptionFilter);

            var scrap = ScrapThis($"{searchQuery}od");

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

            if (doc.InnerHtml.Contains("jest pusta"))
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            int pages = PageProvider(SiteType.Adresowo).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.Adresowo);
            result.Change(o => o.Deal = userSubscriptionFilter.Deal);
            result.Change(o => o.CityId = userSubscriptionFilter.CityId);
            result.Change(o => o.Property = userSubscriptionFilter.Property);

            if (!(userSubscriptionFilter.DistrictId is null))
            {
                result.Change(o => o.DistrictId = userSubscriptionFilter.DistrictId);
            }

            return new ScrappResponse
            {
                Offers = result
            };
        }
    }
}