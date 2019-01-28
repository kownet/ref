using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using System;
using System.Collections.Generic;

namespace Ref.Sites
{
    public class AdresowoSite : BaseSite, ISite
    {
        public AdresowoSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.Adresowo, filterProvider).Get();

            var newest = filterProvider.Newest() == 1 ? "od" : string.Empty;

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                var page = 1;

                while (Element.IsPresent(driver, By.ClassName("offer-list")))
                {
                    driver.Navigate().GoToUrl($"{searchQuery}{page}{newest}");

                    var listing = driver.FindElement(By.ClassName("offer-list"));

                    var articles = listing.FindElements(By.TagName("tr"));

                    if (articles.AnyAndNotNull())
                    {
                        foreach (var article in articles)
                        {
                            string IdE = article.GetAttribute("id");
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

                            if (!string.IsNullOrWhiteSpace(IdE))
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

                        page++;
                    }
                }

                driver.Close();
            }
            return result;
        }
    }
}