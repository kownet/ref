using HtmlAgilityPack;
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
using System.Text.RegularExpressions;

namespace Ref.Sites.Scrapper
{
    public class Morizon : SiteToScrapp, ISiteToScrapp
    {
        public Morizon(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            var searchQuery = QueryStringProvider(SiteType.Morizon).Get(city, dealType);

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
            result.Change(o => o.Deal = dealType);
            result.Change(o => o.CityId = city.Id);

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

        #region Standalone
        public SiteResponse Search(SearchFilter filter)
        {
            var result = new List<Ad>();

            var searchQuery = QueryStringProvider(SiteType.Morizon).Get(filter);

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

            var noResult = doc.CssSelect(".message-title").FirstOrDefault();

            string regex = @"[^0-9 ,.-]";

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Morizon).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}&page={i}").HtmlNode;

                var articles = doc.CssSelect(".row--property-list");

                if (!(articles is null))
                {
                    if (articles.AnyAndNotNull())
                    {
                        foreach (var article in articles)
                        {
                            if (!string.IsNullOrWhiteSpace(article.ByAttribute("data-id")))
                            {
                                var ad = new Ad
                                {
                                    Id = article.ByAttribute("data-id"),
                                    Header = article.ByClass("single-result__title").Replace("nbsp", " "),
                                    Price = article.ByClass("single-result__price", @"[^0-9,.-]"),
                                    PricePerMeter = article.ByClass("single-result__price--currency", @"[^0-9 ,.-]"),
                                    SiteType = SiteType.Morizon
                                };

                                var url = article.CssSelect(".property_link").FirstOrDefault();

                                if (!(url is null))
                                {
                                    ad.Url = url.ByAttribute("href");
                                }

                                var info = article.CssSelect(".info-description").FirstOrDefault();

                                if (!(info is null))
                                {
                                    var elements = info.CssSelect("li");

                                    if (!(elements is null))
                                    {
                                        if (elements.AnyAndNotNull())
                                        {
                                            ad.Rooms = Regex.Replace(elements.First().InnerText, regex, string.Empty).Trim();
                                            ad.Area = Regex.Replace(elements.SecondLast().InnerText, regex, string.Empty).Trim();
                                        }
                                    }
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
        #endregion
    }
}