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
    public class DomiportaSite : BaseSite, ISite
    {
        public DomiportaSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = new DomiportaQueryString(filter).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = new DomiportaPagination().Get(driver);

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}&PageNumber={i}");

                    if (Element.IsPresent(driver, By.ClassName("listing")))
                    {
                        var listing = driver.FindElement(By.ClassName("listing"));

                        if (Element.IsPresent(listing, By.TagName("article")))
                        {
                            var articles = listing.FindElements(By.TagName("article"));

                            if (articles.AnyAndNotNull())
                            {
                                foreach (var article in articles)
                                {
                                    string IdE = string.Empty;
                                    string UrlE = string.Empty;
                                    string HeaderE = string.Empty;
                                    string PriceE = string.Empty;
                                    string RoomsE = string.Empty;
                                    string AreaE = string.Empty;
                                    string PricePerMeterE = string.Empty;

                                    if (Element.IsPresent(article, By.ClassName("sneakpeak__pin")))
                                    {
                                        IdE = article.FindElement(By.ClassName("sneakpeak__pin"))
                                            .FindElement(By.TagName("input"))
                                            .GetAttribute("value");
                                    }

                                    if (Element.IsPresent(article, By.ClassName("sneakpeak__picture_container")))
                                    {
                                        var el = article.FindElement(By.ClassName("sneakpeak__picture_container"));

                                        UrlE = el.GetAttribute("href");
                                        HeaderE = el.GetAttribute("title");
                                    }

                                    if (Element.IsPresent(article, By.ClassName("sneakpeak__details_price")))
                                        PriceE = article.FindElement(By.ClassName("sneakpeak__details_price")).Text;

                                    if (Element.IsPresent(article, By.ClassName("sneakpeak__details_item--area")))
                                        AreaE = article.FindElement(By.ClassName("sneakpeak__details_item--area")).Text;

                                    if (Element.IsPresent(article, By.ClassName("sneakpeak__details_item--price")))
                                        PricePerMeterE = article.FindElement(By.ClassName("sneakpeak__details_item--price")).Text;

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
                                            SiteType = SiteType.DomiPorta
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