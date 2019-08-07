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
    public class Gumtree : SiteToScrapp, ISiteToScrapp
    {
        public Gumtree(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider,
            IDistrictRepository districtRepository)
            : base(appProvider, pageProvider, queryStringProvider, districtRepository)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType, District district)
        {
            if (!city.IsGumtreeAvailableForSale && dealType == DealType.Sale)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>()
                };
            }

            if (!city.IsGumtreeAvailableForRent && dealType == DealType.Rent)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>()
                };
            }

            var searchQuery = QueryStringProvider(SiteType.Gumtree).Get(city, dealType, district);

            var code = dealType == DealType.Sale ? city.GtCodeSale : city.GtCodeRent;

            if (!(district is null))
            {
                code = dealType == DealType.Sale ? $"{district.GtCodeSale}" : district.GtCodeRent;
            }

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

            //int.TryParse(doc.ByClass("count", @"[^0-9]"), out int count);

            //if (count == 0)
            //{
            //    return new ScrappResponse
            //    {
            //        Offers = new List<Offer>(),
            //        ThereAreNoRecords = true
            //    };
            //}

            int pages = PageProvider(SiteType.Gumtree).Get(doc, code);

            var result = Crawl(pages, searchQuery, doc, code);

            result.Change(o => o.Site = SiteType.Gumtree);
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

        public ScrappResponse Scrapp(UserSubscriptionFilter userSubscriptionFilter)
        {
            if (userSubscriptionFilter.Deal == DealType.Rent)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>()
                };
            }

            var searchQuery = QueryStringProvider(SiteType.Gumtree).Get(userSubscriptionFilter);

            var code = userSubscriptionFilter.Deal == DealType.Sale ? userSubscriptionFilter.GumtreeCitySale : "";

            if (!(userSubscriptionFilter.DistrictId is null))
            {
                code = userSubscriptionFilter.Deal == DealType.Sale ? $"{userSubscriptionFilter.GumtreeDistrictSale}" : "";
            }

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

            int pages = PageProvider(SiteType.Gumtree).Get(doc, code);

            var result = Crawl(pages, searchQuery, doc, code);

            result.Change(o => o.Site = SiteType.Gumtree);
            result.Change(o => o.Deal = userSubscriptionFilter.Deal);
            result.Change(o => o.CityId = userSubscriptionFilter.CityId);
            result.Change(o => o.Property = userSubscriptionFilter.Property);
            result.Change(o => o.IsFromPrivate = userSubscriptionFilter.AllowPrivate);
            result.Change(o => o.IsFromAgency = userSubscriptionFilter.AllowFromAgency);

            if (!(userSubscriptionFilter.DistrictId is null))
            {
                result.Change(o => o.DistrictId = userSubscriptionFilter.DistrictId);
            }

            return new ScrappResponse
            {
                Offers = result
            };
        }

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc, string code)
        {
            var result = new List<Offer>();

            /// order by desc, no need to grab all sites
            if (pages > AppProvider.Pages())
                pages = AppProvider.Pages();

            for (int i = 1; i <= pages; i++)
            {
                var sq = searchQuery.Replace($"{code}1", $"page-{i}/{code}{i}");

                var scrap = ScrapThis($@"{sq}");

                if (!scrap.Succeed)
                    return result;

                doc = scrap.HtmlNode;

                var listing = doc.CssSelect(".tileV1");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var article in listing)
                        {
                            var ad = new Offer
                            {
                                DateAdded = DateTime.Now
                            };

                            if (int.TryParse(article.ByClass("ad-price", @"[^0-9,.-]"), out int price))
                            {
                                ad.Price = price;
                            }

                            var link = article.CssSelect(".href-link").FirstOrDefault();

                            if (!(link is null))
                            {
                                ad.Url = $"https://www.gumtree.pl{link.ByAttribute("href")}";
                                ad.Header = link.InnerText;

                                if (!string.IsNullOrWhiteSpace(ad.Url))
                                {
                                    ad.SiteOfferId = ad.Url.Split("/").Last();
                                }
                            }

                            if (ad.IsValidToAdd())
                                result.Add(ad);
                        }
                    }
                }
            }

            return result;
        }
    }
}