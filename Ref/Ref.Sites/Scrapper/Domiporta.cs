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
using System.Web;

namespace Ref.Sites.Scrapper
{
    public class Domiporta : SiteToScrapp, ISiteToScrapp
    {
        public Domiporta(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType, District district)
        {
            var searchQuery = QueryStringProvider(SiteType.DomiPorta).Get(city, dealType, district);

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

            if (!(district is null))
            {
                result.Change(o => o.DistrictId = district.Id);
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
                                    if (areaRaw.Length >= 2)
                                    {
                                        areaRaw = areaRaw.Replace(",", "").Substring(0, 2);

                                        if (int.TryParse(areaRaw, out int area))
                                        {
                                            ad.Area = area;
                                        }
                                    }
                                }

                                var ppms = article.ByClass("sneakpeak__details_item--price", @"[^0-9,.-]");

                                if (!string.IsNullOrWhiteSpace(ppms))
                                {
                                    ppms = ppms.Remove(ppms.Length - 1);

                                    if (int.TryParse(ppms, out int ppm))
                                    {
                                        ad.PricePerMeter = ppm;
                                    }
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

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            throw new NotImplementedException();
        }
    }
}