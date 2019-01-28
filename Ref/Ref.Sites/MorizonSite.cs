using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ref.Sites
{
    public class MorizonSite : BaseSite, ISite
    {
        public MorizonSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.Morizon, filterProvider).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = 1;

                if (Element.IsPresent(driver, By.ClassName("mz-pagination-number")))
                {
                    var pagesElementText = driver.FindElement(By.ClassName("mz-pagination-number"));

                    var pagesElements = pagesElementText.FindElements(By.TagName("li"));

                    if (pagesElements.AnyAndNotNull())
                    {
                        if (int.TryParse(pagesElements.SecondLast().Text, out pages))
                        {

                        }
                    }
                }

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}&page={i}");

                    if (Element.IsPresent(driver, By.XPath("//*[@id=\"contentPage\"]/div/div/div/div/section")))
                    {
                        var listing = driver.FindElement(By.XPath("//*[@id=\"contentPage\"]/div/div/div/div/section"));

                        var articles = listing.FindElements(By.ClassName("row--property-list"));

                        if (articles.AnyAndNotNull())
                        {
                            foreach (var article in articles)
                            {
                                var IdE = article.GetAttribute("data-id");

                                string UrlE = string.Empty;
                                string HeaderE = string.Empty;
                                string PriceE = string.Empty;
                                string RoomsE = string.Empty;
                                string AreaE = string.Empty;
                                string PricePerMeterE = string.Empty;

                                if (Element.IsPresent(article, By.ClassName("property_link")))
                                    UrlE = article.FindElement(By.ClassName("property_link")).GetAttribute("href");

                                if (Element.IsPresent(article, By.ClassName("single-result__title")))
                                    HeaderE = article.FindElement(By.ClassName("single-result__title")).Text;

                                if (Element.IsPresent(article, By.ClassName("single-result__price")))
                                    PriceE = article.FindElement(By.ClassName("single-result__price")).Text;

                                if (Element.IsPresent(article, By.ClassName("single-result__price--currency")))
                                    PricePerMeterE = article.FindElement(By.ClassName("single-result__price--currency")).Text;

                                if (Element.IsPresent(article, By.ClassName("info-description")))
                                {
                                    var infos = driver.FindElement(By.ClassName("info-description"));

                                    var elements = infos.FindElements(By.TagName("li"));

                                    if (elements.AnyAndNotNull())
                                    {
                                        RoomsE = elements.First().Text;
                                        AreaE = elements.SecondLast().Text;
                                    }
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