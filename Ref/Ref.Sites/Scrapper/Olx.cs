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
    public class Olx : SiteToScrapp, ISiteToScrapp
    {
        public Olx(
            IAppProvider appProvider,
            Func<SiteType, IPages> pageProvider,
            Func<SiteType, IQueryString> queryStringProvider)
            : base(appProvider, pageProvider, queryStringProvider)
        {
        }

        public ScrappResponse Scrapp(City city, DealType dealType)
        {
            var searchQuery = QueryStringProvider(SiteType.Olx).Get(city, dealType);

            var doc = ScrapThis(searchQuery);

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

            result.Change(o => o.SiteType = SiteType.Olx);
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

            var searchQuery = QueryStringProvider(SiteType.Olx).Get(filter);

            var doc = ScrapThis(searchQuery);

            var noResult = doc.CssSelect(".emptynew ").FirstOrDefault();

            if (noResult != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description()
                };
            }

            var banned = doc.CssSelect(".message").FirstOrDefault();

            if (banned != null)
            {
                return new SiteResponse
                {
                    FilterName = filter.Name,
                    Advertisements = result,
                    FilterDesc = filter.Description(),
                    WeAreBanned = true
                };
            }

            int pages = PageProvider(SiteType.Olx).Get(doc);

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}page={i}");

                var listing = doc.CssSelect(".offer-wrapper");

                if (!(listing is null))
                {
                    if (listing.AnyAndNotNull())
                    {
                        foreach (var offer in listing)
                        {
                            var ad = new Ad
                            {
                                SiteType = SiteType.Olx
                            };

                            var table = offer.CssSelect("table").FirstOrDefault();

                            if(!(table is null))
                            {
                                ad.Id = table.ByAttribute("data-id");

                                var link = table.CssSelect(".linkWithHash").FirstOrDefault();

                                if(!(link is null))
                                {
                                    var url = link.ByAttribute("href");

                                    ad.Url = url;
                                    ad.Header = Parse(url);
                                }

                                ad.Price = table.ByClass("price", @"[^0-9,.-]");
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

        private List<Offer> Crawl(int pages, string searchQuery, HtmlNode doc)
        {
            var result = new List<Offer>();

            for (int i = 1; i <= pages; i++)
            {
                doc = ScrapThis($@"{searchQuery}page={i}");

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

                            if (ad.IsValidToAdd())
                                result.Add(ad);
                        }
                    }
                }
            }

            return result;
        }

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
    }
}