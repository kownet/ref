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
    public class AdresowoSite : BaseSite, ISite
    {
        public AdresowoSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = new AdresowoQueryString(filter).Get();

            var newest = filter.Newest == 1 ? "od" : string.Empty;

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = new AdresowoPagination().Get(driver);

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($"{searchQuery}{i}{newest}");

                    if (Element.IsPresent(driver, By.ClassName("offer-list")))
                    {
                        var listing = driver.FindElement(By.ClassName("offer-list"));

                        if (Element.IsPresent(listing, By.TagName("tr")))
                        {
                            var articles = listing.FindElements(By.TagName("tr"));

                            if (articles.AnyAndNotNull())
                            {
                                foreach (var article in articles)
                                {
                                    string IdE = !string.IsNullOrWhiteSpace(article.GetAttribute("id")) ? article.GetAttribute("id").Remove(0, 1) : string.Empty;
                                    string UrlE = $"https://adresowo.pl/o/{IdE}";
                                    string HeaderE = string.Empty;
                                    string PriceE = string.Empty;
                                    string RoomsE = string.Empty;
                                    string AreaE = string.Empty;
                                    string PricePerMeterE = string.Empty;

                                    if (Element.IsPresent(article, By.ClassName("address")))
                                        HeaderE = article.FindElement(By.ClassName("address")).Text;

                                    if (Element.IsPresent(article, By.ClassName("price")))
                                        PriceE = article.FindElement(By.ClassName("price")).Text;

                                    if (Element.IsPresent(article, By.ClassName("price-per-unit")))
                                        PricePerMeterE = article.FindElement(By.ClassName("price-per-unit")).Text;

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
                                            SiteType = SiteType.Adresowo
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