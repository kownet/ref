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
    public class GratkaSite : BaseSite, ISite
    {
        public GratkaSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.Gratka, filterProvider).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = 1;

                if (Element.IsPresent(driver, By.ClassName("pagination")))
                {
                    var pagesElementText = driver.FindElement(By.ClassName("pagination"));

                    if (Element.IsPresent(pagesElementText, By.TagName("input")))
                    {
                        var input = pagesElementText.FindElement(By.TagName("input"));

                        if (int.TryParse(input.GetAttribute("max"), out pages))
                        {

                        }
                    }
                }

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}&page={i}");

                    if (Element.IsPresent(driver, By.ClassName("content__listing")))
                    {
                        var listing = driver.FindElement(By.ClassName("content__listing"));

                        if(Element.IsPresent(listing, By.TagName("article")))
                        {
                            var articles = listing.FindElements(By.TagName("article"));

                            if (articles.AnyAndNotNull())
                            {
                                foreach (var article in articles)
                                {
                                    var IdE = article.GetAttribute("id");
                                    var UrlE = article.GetAttribute("data-href");

                                    string HeaderE = string.Empty;
                                    string PriceE = string.Empty;
                                    string RoomsE = string.Empty;
                                    string AreaE = string.Empty;
                                    string PricePerMeterE = string.Empty;

                                    if (Element.IsPresent(article, By.ClassName("teaser__anchor")))
                                        HeaderE = article.FindElement(By.ClassName("teaser__anchor")).Text;

                                    if (Element.IsPresent(article, By.ClassName("teaser__price")))
                                    {
                                        var p = article.FindElement(By.ClassName("teaser__price")).Text;

                                        var splitted = p.Split("\r\n");

                                        PriceE = splitted[0];
                                        PricePerMeterE = splitted[1];
                                    }

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
                                            SiteType = SiteType.Gratka
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
            return result;
        }
    }
}