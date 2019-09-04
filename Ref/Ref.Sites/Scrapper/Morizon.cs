using HtmlAgilityPack;
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
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper
{
    public class Morizon : SiteToScrapp, ISiteToScrapp
    {
        public Morizon(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            var searchQuery = QueryStringProvider(SiteType.Morizon).Get(userSubscriptionFilter);

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

            if (doc.InnerHtml.Contains("Ta strona została zablokowana."))
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    WeAreBanned = true
                };
            }

            var noResult = doc.CssSelect(".message-title").FirstOrDefault();

            if (noResult != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            int pages = PageProvider(SiteType.Morizon).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.Morizon);
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

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc)
        {
            var result = new List<Offer>();

            string regex = @"[^0-9,.-]";

            for (int i = 1; i <= pages; i++)
            {
                var scrap = ScrapThis($@"{searchQuery}&page={i}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

                var articles = doc.CssSelect(".row--property-list");

                if (!(articles is null))
                {
                    if (articles.AnyAndNotNull())
                    {
                        foreach (var article in articles)
                        {
                            if (!string.IsNullOrWhiteSpace(article.ByAttribute("data-id")))
                            {
                                var ad = new Offer
                                {
                                    SiteOfferId = article.ByAttribute("data-id"),
                                    Header = article.ByClass("single-result__title").Replace("nbsp", " "),
                                    DateAdded = DateTime.Now,
                                };

                                var url = article.CssSelect(".property_link").FirstOrDefault();

                                if (!(url is null))
                                {
                                    ad.Url = url.ByAttribute("href");
                                }

                                if (int.TryParse(article.ByClass("single-result__price", @"[^0-9,.-]"), out int price))
                                {
                                    ad.Price = price;
                                }

                                var ppms = article.ByClass("single-result__price--currency", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(ppms))
                                {
                                    if (ppms.Contains(","))
                                    {
                                        var ppmsSplitted = ppms.Split(",")[0];

                                        if (int.TryParse(ppmsSplitted, out int ppm))
                                        {
                                            ad.PricePerMeter = ppm;
                                        }
                                    }
                                }

                                var info = article.CssSelect(".info-description").FirstOrDefault();

                                if (!(info is null))
                                {
                                    var elements = info.CssSelect("li");

                                    if (!(elements is null))
                                    {
                                        if (elements.AnyAndNotNull())
                                        {
                                            foreach (var element in elements)
                                            {
                                                if (!(element is null))
                                                {
                                                    if (!string.IsNullOrWhiteSpace(element.InnerText))
                                                    {
                                                        if (element.InnerText.Contains(" m²"))
                                                        {
                                                            if (int.TryParse(Regex.Replace(element.InnerText, regex, string.Empty).Trim(), out int a))
                                                            {
                                                                ad.Area = a;
                                                            }
                                                        }

                                                        if (element.InnerText.Contains("pokoje") || element.InnerText.Contains("pokój"))
                                                        {
                                                            if (int.TryParse(Regex.Replace(element.InnerText, regex, string.Empty).Trim(), out int r))
                                                            {
                                                                ad.Rooms = r;
                                                            }
                                                        }
                                                    }
                                                }
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
    }
}