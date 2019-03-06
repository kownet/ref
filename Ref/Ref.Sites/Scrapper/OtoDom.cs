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

namespace Ref.Sites.Scrapper
{
    public class OtoDom : SiteToScrapp, ISiteToScrapp
    {
        public OtoDom(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            var searchQuery = QueryStringProvider(SiteType.OtoDom).Get(city, dealType);

            var doc = ScrapThis(searchQuery);

            var noResult = doc.CssSelect(".search-location-extended-warning").FirstOrDefault();

            if (noResult != null)
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

            var searchQuery = QueryStringProvider(SiteType.OtoDom).Get(filter);

            var doc = ScrapThis(searchQuery);

            var noResult = doc.CssSelect(".search-location-extended-warning").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.OtoDom).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}page={i}");

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
                                var ad = new Ad
                                {
                                    Id = article.ByAttribute("data-tracking-id"),
                                    Url = article.ByAttribute("data-url"),
                                    Header = article.ByClass("offer-item-title"),
                                    Price = article.ByClass("offer-item-price", @"[^0-9,.-]"),
                                    Rooms = article.ByClass("offer-item-rooms", @"[^0-9 ,.-]"),
                                    Area = article.ByClass("offer-item-area", @"[^0-9 ,.-]"),
                                    PricePerMeter = article.ByClass("offer-item-price-per-m", @"[^0-9 ,.-]").RemoveLastIf("2"),
                                    SiteType = SiteType.OtoDom
                                };

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

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}page={i}");

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