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
    public class Olx : SiteToScrapp, ISiteToScrapp
    {
        public Olx(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType, District district)
        {
            var searchQuery = QueryStringProvider(SiteType.Olx).Get(city, dealType, district);

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

            var noResult = doc.CssSelect(".emptynew ").FirstOrDefault();

            if (noResult != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            var banned = doc.CssSelect(".message").FirstOrDefault();

            if (banned != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    WeAreBanned = true
                };
            }

            int pages = PageProvider(SiteType.Olx).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.Olx);
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
                var scrap = ScrapThis($@"{searchQuery}&page={i}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

                var listing = doc.CssSelect(".offer-wrapper");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var offer in listing)
                        {
                            var ad = new Offer
                            {
                                DateAdded = DateTime.Now
                            };

                            var table = offer.CssSelect("table").FirstOrDefault();

                            if (!(table is null))
                            {
                                ad.SiteOfferId = table.ByAttribute("data-id");

                                var link = table.CssSelect(".linkWithHash").FirstOrDefault();

                                if (!(link is null))
                                {
                                    var url = link.ByAttribute("href");

                                    ad.Url = url;
                                    ad.Header = Parse(url);
                                }

                                if (int.TryParse(table.ByClass("price", @"[^0-9,.-]"), out int price))
                                {
                                    ad.Price = price;
                                }
                            }

                            if (ad.IsValidToAdd() && !ad.Url.Contains("https://www.otodom.pl"))
                                result.Add(ad);
                        }
                    }
                }
            }

            return result;
        }

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            var searchQuery = QueryStringProvider(SiteType.Olx).Get(userSubscriptionFilter);

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

            var noResult = doc.CssSelect(".emptynew ").FirstOrDefault();

            if (noResult != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            var banned = doc.CssSelect(".message").FirstOrDefault();

            if (banned != null)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    WeAreBanned = true
                };
            }

            int pages = PageProvider(SiteType.Olx).Get(doc);

            var result = Crawl(pages, searchQuery, doc);

            result.Change(o => o.Site = SiteType.Olx);
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

        #region Private
        private readonly string _olxBanner = "https://www.olx.pl/oferta/";
        private readonly string _otodomBanner = "https://www.otodom.pl/oferta/";

        private string Parse(string header)
        {
            if (header.Contains(_olxBanner))
            {
                var link = header.Replace(_olxBanner, string.Empty);

                var split = link.Split('-');

                Array.Resize(ref split, split.Length - 2);

                return string.Join(" ", split).FirstCharToUpper();
            }

            if (header.Contains(_otodomBanner))
            {
                var link = header.Replace(_otodomBanner, string.Empty);

                var split = link.Split('-');

                Array.Resize(ref split, split.Length - 1);

                return string.Join(" ", split).FirstCharToUpper();
            }

            else return string.Empty;
        }
        #endregion
    }
}