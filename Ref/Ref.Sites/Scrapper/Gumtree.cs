using HtmlAgilityPack;
using Ref.Data.Models;
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
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            var searchQuery = QueryStringProvider(SiteType.Gumtree).Get(city, dealType);

            var code = dealType == DealType.Sale ? city.GtCodeSale : city.GtCodeRent;

            var doc = ScrapThis(searchQuery);

            int.TryParse(doc.ByClass("count", @"[^0-9]"), out int count);

            if (count == 0)
            {
                return new ScrappResponse
                {
                    Offers = new List<Offer>(),
                    ThereAreNoRecords = true
                };
            }

            int pages = PageProvider(SiteType.Gumtree).Get(doc, code);

            var result = Crawl(pages, searchQuery, doc, code);

            result.Change(o => o.SiteType = SiteType.Gumtree);
            result.Change(o => o.DealType = dealType);
            result.Change(o => o.CityId = city.Id);

            return new ScrappResponse
            {
                Offers = result
            };
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = QueryStringProvider(SiteType.Gumtree).Get(filter);

            var code = FilterResolver.Code(filter);

            var doc = ScrapThis(searchQuery);

            int.TryParse(doc.ByClass("count", @"[^0-9]"), out int count);

            if (count == 0)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            int pages = PageProvider(SiteType.Gumtree).Get(doc, code);

            for (int i = 1; i <= pages; i++)
            {
                var sq = searchQuery.Replace($"{code}1", $"page-{i}/{code}{i}");

                doc = ScrapThis($@"{sq}");

                var listing = doc.CssSelect(".result-link");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var article in listing)
                        {
                            var ad = new Ad
                            {
                                SiteType = SiteType.Gumtree,
                                Price = article.ByClass("amount", @"[^0-9,.-]")
                            };

                            var link = article.CssSelect(".href-link").FirstOrDefault();

                            if (!(link is null))
                            {
                                ad.Url = $"https://www.gumtree.pl{link.ByAttribute("href")}";
                                ad.Header = link.InnerText;

                                if (!string.IsNullOrWhiteSpace(ad.Url))
                                {
                                    ad.Id = ad.Url.Split("/").Last();
                                }
                            }

                            if (ad.IsValid())
                                result.Add(ad);
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

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc, string code)
        {
            var result = new List<Offer>();

            for (int i = 1; i <= pages; i++)
            {
                var sq = searchQuery.Replace($"{code}1", $"page-{i}/{code}{i}");

                doc = ScrapThis($@"{sq}");

                var listing = doc.CssSelect(".result-link");

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

                            if (int.TryParse(article.ByClass("amount", @"[^0-9,.-]"), out int price))
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