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
    public class OtoDom : SiteToScrapp, ISiteToScrapp
    {
        public OtoDom(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            var searchQuery = QueryStringProvider(SiteType.OtoDom).Get(userSubscriptionFilter);

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

            var noResult = doc.CssSelect(".search-location-extended-warning").FirstOrDefault();

            if (!(noResult is null))
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            int pages = PageProvider(SiteType.OtoDom).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.OtoDom);
            result.Change(o => o.Deal = userSubscriptionFilter.Deal);
            result.Change(o => o.CityId = userSubscriptionFilter.CityId);
            result.Change(o => o.Property = userSubscriptionFilter.Property);

            if (userSubscriptionFilter.AllowPrivate && !userSubscriptionFilter.AllowFromAgency)
            {
                result.Change(o => o.IsFromPrivate = userSubscriptionFilter.AllowPrivate);
                result.Change(o => o.IsFromAgency = userSubscriptionFilter.AllowFromAgency);
            }

            if (!(userSubscriptionFilter.DistrictId is null))
            {
                result.Change(o => o.DistrictId = userSubscriptionFilter.DistrictId);
            }

            return new ScrappResponse
            {
                Offers = result
            };
        }

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc)
        {
            var result = new List<Offer>();

            for (int i = 1; i <= pages; i++)
            {
                var scrap = ScrapThis($@"{searchQuery}&page={i}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

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
                                var ad = new Offer
                                {
                                    SiteOfferId = article.ByAttribute("data-tracking-id"),
                                    Url = article.ByAttribute("data-url"),
                                    Header = article.ByClass("offer-item-title"),
                                    DateAdded = DateTime.Now
                                };

                                if (int.TryParse(article.ByClass("offer-item-price", @"[^0-9,.-]"), out int price))
                                {
                                    ad.Price = price;
                                }

                                var areaRaw = article.ByClass("offer-item-area", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(areaRaw))
                                {
                                    if (areaRaw.Contains(","))
                                    {
                                        areaRaw = areaRaw.Replace(",", "");

                                        var length = areaRaw.Length;

                                        if (length == 4 || length == 3)
                                            areaRaw = areaRaw.Substring(0, 2);

                                        if (length == 5)
                                            areaRaw = areaRaw.Substring(0, 3);

                                        if (int.TryParse(areaRaw, out int area))
                                        {
                                            ad.Area = area;
                                        }
                                    }
                                    else
                                    {
                                        var length = areaRaw.Length;

                                        if (length >= 2 && length <= 3)
                                        {
                                            areaRaw = areaRaw.Substring(0, length);

                                            if (int.TryParse(areaRaw, out int area))
                                            {
                                                ad.Area = area;
                                            }
                                        }
                                    }
                                }

                                if (int.TryParse(article.ByClass("offer-item-rooms", @"[^0-9,.-]"), out int rooms))
                                {
                                    ad.Rooms = rooms;
                                }

                                if (int.TryParse(article.ByClass("offer-item-price-per-m", @"[^0-9,.-]"), out int ppm))
                                {
                                    ad.PricePerMeter = ppm;
                                }

                                var privOrAgency = article.CssSelect(".offer-item-details-bottom").FirstOrDefault();

                                if (!(privOrAgency is null))
                                {
                                    var privOrAgencyStr = privOrAgency.InnerText;

                                    if (!string.IsNullOrWhiteSpace(privOrAgencyStr))
                                    {
                                        privOrAgencyStr = privOrAgencyStr.Trim();

                                        if (string.Equals("Oferta prywatna", privOrAgencyStr))
                                        {
                                            ad.IsFromPrivate = true;
                                            ad.IsFromAgency = false;
                                        }
                                        else
                                        {
                                            ad.IsFromPrivate = false;
                                            ad.IsFromAgency = true;
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
    }
}