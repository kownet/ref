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
    public class GumtreeSite : BaseSite, ISite
    {
        public GumtreeSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.Gumtree, filterProvider).Get();

            var code = FilterResolver.Code(filterProvider);

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = 1;

                if (Element.IsPresent(driver, By.ClassName("pagination")))
                {
                    var pagesElementText = driver.FindElement(By.ClassName("pagination"));

                    if (Element.IsPresent(pagesElementText, By.ClassName("after")))
                    {
                        var pagesElements = pagesElementText.FindElement(By.ClassName("after"));

                        if (Element.IsPresent(pagesElements, By.TagName("a")))
                        {
                            var aElements = pagesElements.FindElements(By.TagName("a"));

                            if (aElements.AnyAndNotNull())
                            {
                                var lastPageLink = aElements.Last().GetAttribute("href");

                                var lastPageValue = lastPageLink.TextAfter(code).First().ToString();

                                if (int.TryParse(lastPageValue, out pages))
                                {

                                }
                            }
                        }
                    }
                }

                for (int i = 1; i <= pages; i++)
                {
                    var sq = searchQuery.Replace($"{code}1", $"page-{i}/{code}{i}");

                    driver.Navigate().GoToUrl($@"{sq}");

                    if(Element.IsPresent(driver, By.ClassName("result-link")))
                    {
                        var articles = driver.FindElements(By.ClassName("result-link"));

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

                                if (Element.IsPresent(article, By.ClassName("href-link")))
                                {
                                    var e = article.FindElement(By.ClassName("href-link"));
                                    UrlE = e.GetAttribute("href");
                                    HeaderE = e.Text;

                                    if (!string.IsNullOrWhiteSpace(UrlE))
                                    {
                                        IdE = UrlE.Split("/").Last();
                                    }
                                }

                                if (Element.IsPresent(article, By.ClassName("amount")))
                                    PriceE = article.FindElement(By.ClassName("amount")).Text;

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
                                        SiteType = SiteType.Gumtree
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