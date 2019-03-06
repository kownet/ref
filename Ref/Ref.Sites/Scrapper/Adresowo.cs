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
    public class Adresowo : SiteToScrapp, ISiteToScrapp
    {
        public Adresowo(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            if (dealType == DealType.Rent)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            var searchQuery = QueryStringProvider(SiteType.Adresowo).Get(city, dealType);

            var doc = ScrapThis(searchQuery, "iso-8859-2");

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

            var searchQuery = QueryStringProvider(SiteType.Adresowo).Get(filter);

            var doc = ScrapThis(searchQuery, "iso-8859-2");

            if (doc.InnerHtml.Contains("jest pusta"))
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Adresowo).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($"{searchQuery}{i}", "iso-8859-2");

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
                                var ad = new Ad
                                {
                                    SiteType = SiteType.Adresowo
                                };

                                ad.Id = article.ByAttribute("data-public-code");
                                ad.Url = $"https://adresowo.pl/o/{ad.Id}";

                                var header = article.CssSelect(".result-info__header").FirstOrDefault();

                                if(!(header is null))
                                {
                                    if(!string.IsNullOrWhiteSpace(header.InnerText))
                                    {
                                        ad.Header = header.InnerText
                                            .Trim()
                                            .Replace("\n", " ")
                                            .Replace("Mieszkanie na sprzedaż", "");
                                    }
                                }

                                var area = article.CssSelect(".result-info__basic").Skip(1).FirstOrDefault();

                                if(!(area is null))
                                {
                                    if (!string.IsNullOrWhiteSpace(area.InnerText))
                                    {
                                        ad.Area = area.InnerText
                                            .Replace("m&sup2;", "")
                                            .Trim();
                                    }
                                }
                                ad.Price = article.ByClass("result-info__price--total", @"[^0-9 ,.-]").Trim().Replace(" ", "");
                                ad.PricePerMeter = article.ByClass("result-info__price--per-sqm", @"[^0-9 ,.-]").RemoveLastIf("2");

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
                doc = ScrapThis($"{searchQuery}{i}", "iso-8859-2");

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

                                if (int.TryParse(article.ByClass("price", @"[^0-9,.-]"), out int price))
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