using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using Ref.Sites.Helpers.Pagination;
using Ref.Sites.Helpers.QueryStrings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ref.Sites
{
    public class OtoDomSite : BaseSite, ISite
    {
        public OtoDomSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = new OtoDomQueryString(filter).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = new OtoDomPagination().Get(driver);

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}page={i}");

                    if (Element.IsPresent(driver, By.ClassName("section-listing__row-content")))
                    {
                        var listing = driver.FindElement(By.ClassName("section-listing__row-content"));

                        if (Element.IsPresent(listing, By.TagName("article")))
                        {
                            var articles = listing.FindElements(By.TagName("article"));

                            if (articles.AnyAndNotNull())
                            {
                                foreach (var article in articles)
                                {
                                    var IdE = article.GetAttribute("data-tracking-id");
                                    var UrlE = article.GetAttribute("data-url");

                                    string HeaderE = string.Empty;
                                    string PriceE = string.Empty;
                                    string RoomsE = string.Empty;
                                    string AreaE = string.Empty;
                                    string PricePerMeterE = string.Empty;

                                    if (Element.IsPresent(article, By.ClassName("offer-item-title")))
                                        HeaderE = article.FindElement(By.ClassName("offer-item-title")).Text;

                                    if (Element.IsPresent(article, By.ClassName("offer-item-price")))
                                        PriceE = article.FindElement(By.ClassName("offer-item-price")).Text;

                                    if (Element.IsPresent(article, By.ClassName("offer-item-rooms")))
                                        RoomsE = article.FindElement(By.ClassName("offer-item-rooms")).Text;

                                    if (Element.IsPresent(article, By.ClassName("offer-item-area")))
                                        AreaE = article.FindElement(By.ClassName("offer-item-area")).Text;

                                    if (Element.IsPresent(article, By.ClassName("offer-item-price-per-m")))
                                        PricePerMeterE = article.FindElement(By.ClassName("offer-item-price-per-m")).Text;

                                    if (!string.IsNullOrWhiteSpace(IdE) && !string.IsNullOrWhiteSpace(UrlE))
                                    {
                                        var ad = new Ad
                                        {
                                            Id = IdE,
                                            Url = UrlE,
                                            Header = HeaderE,
                                            Price = PriceE,
                                            Rooms = RoomsE,
                                            Area = AreaE,
                                            PricePerMeter = PricePerMeterE,
                                            SiteType = SiteType.OtoDom
                                        };

                                        result.Add(ad);
                                    }
                                }
                            }
                        }
                    }
                }
                driver.Close();
            }
            return new SiteResponse
            {
                FilterName = filter.Name,
                Advertisements = result
            };
        }
    }
}