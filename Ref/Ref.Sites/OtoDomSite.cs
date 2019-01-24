using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ref.Sites
{
    public class OtoDomSite : ISite
    {
        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var options = new ChromeOptions();

            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");

            var service = ChromeDriverService.CreateDefaultService(Path.Combine(@"C:\!Scrapper\drivers"));

            service.SuppressInitialDiagnosticInformation = false;
            service.HideCommandPromptWindow = true;

            var searchQuery = QueryStringBuilder.OtoDom(filterProvider);

            using (var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(120)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = 1;

                if (Element.IsPresent(driver, By.XPath("//*[@id=\"pagerForm\"]/ul/li[1]/strong")))
                {
                    var pagesElementText = driver.FindElement(By.XPath("//*[@id=\"pagerForm\"]/ul/li[1]/strong")).Text;

                    if (int.TryParse(pagesElementText, out pages))
                    {

                    }
                }

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}page={i}");

                    if (Element.IsPresent(driver, By.XPath("//*[@id=\"body-container\"]/div/div/div[2]")))
                    {
                        var listing = driver.FindElement(By.XPath("//*[@id=\"body-container\"]/div/div/div[2]"));

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
                                        PricePerMeter = PricePerMeterE
                                    };

                                    result.Add(ad);
                                }
                            }
                        }
                    }
                }

                driver.Close();
            }

            return result;
        }
    }
}