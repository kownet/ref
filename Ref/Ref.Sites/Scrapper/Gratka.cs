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

namespace Ref.Sites.Scrapper
{
    public class Gratka : SiteToScrapp, ISiteToScrapp
    {
        public Gratka(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            var searchQuery = QueryStringProvider(SiteType.Gratka).Get(userSubscriptionFilter);

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

            if (doc.InnerHtml.Contains("tymczasowo zablokowany"))
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    WeAreBanned = true
                };
            }

            var noResult = doc.CssSelect(".content__emptyListInfo").FirstOrDefault();

            if (noResult != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            int pages = PageProvider(SiteType.Gratka).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.Gratka);
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

            /// order by desc, no need to grab all sites
            if (pages > AppProvider.Pages())
                pages = AppProvider.Pages();

            for (int i = 1; i <= pages; i++)
            {
                var scrap = ScrapThis($@"{searchQuery}&page={i}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

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
                                var ad = new Offer
                                {
                                    SiteOfferId = article.ByAttribute("id"),
                                    Url = article.ByAttribute("data-href"),
                                    DateAdded = DateTime.Now
                                };

                                ad.Header = article.ByClass("teaser__anchor");

                                var price = article.CssSelect($".teaser__price").FirstOrDefault();

                                if (!(price is null))
                                {
                                    if (int.TryParse(price.FirstChild != null ? price.FirstChild.InnerHtml.Trim().Replace(" ", string.Empty) : string.Empty, out int pricee))
                                    {
                                        ad.Price = pricee;
                                    }
                                }

                                var pricepm = article.ByClass("teaser__priceAdditional", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(pricepm))
                                {
                                    pricepm = pricepm.RemoveLastIf("2");

                                    if (int.TryParse(pricepm, out int ppm))
                                    {
                                        ad.PricePerMeter = ppm;
                                    }
                                }

                                var param = article.CssSelect(".teaser__params").FirstOrDefault();

                                if (!(param is null))
                                {
                                    var lis = param.CssSelect("li");

                                    if (lis.AnyAndNotNull())
                                    {
                                        foreach (var li in lis)
                                        {
                                            var areaRaw = li;

                                            if (!(areaRaw is null))
                                            {
                                                if (!string.IsNullOrWhiteSpace(areaRaw.InnerText))
                                                {
                                                    if (areaRaw.InnerText.Contains("Powierzchnia w m2: "))
                                                    {
                                                        var nmb = areaRaw.InnerText.Replace("Powierzchnia w m2: ", "");

                                                        if (nmb.Contains("."))
                                                        {
                                                            nmb = nmb.Replace(".", "");

                                                            var length = nmb.Length;

                                                            if (length == 4 || length == 3)
                                                                nmb = nmb.Substring(0, 2);

                                                            if (length == 5)
                                                                nmb = nmb.Substring(0, 3);

                                                            if (int.TryParse(nmb, out int area))
                                                            {
                                                                ad.Area = area;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (int.TryParse(nmb, out int a))
                                                            {
                                                                ad.Area = a;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            var roomsRaw = li;

                                            if (!(roomsRaw is null))
                                            {
                                                if (!string.IsNullOrWhiteSpace(roomsRaw.InnerText))
                                                {
                                                    if (roomsRaw.InnerText.Contains("Liczba pokoi: "))
                                                    {
                                                        if (int.TryParse(roomsRaw.InnerText.Replace("Liczba pokoi: ", ""), out int r))
                                                        {
                                                            ad.Rooms = r;
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