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
    public class OlxSite : BaseSite, ISite
    {
        public OlxSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = new OlxQueryString(filter).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = new OlxPagination().Get(driver);

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}page={i}");

                    if (Element.IsPresent(driver, By.ClassName("offer-wrapper")))
                    {
                        var offers = driver.FindElements(By.ClassName("offer-wrapper"));

                        if (offers.AnyAndNotNull())
                        {
                            foreach (var offer in offers)
                            {
                                string IdE = string.Empty;
                                string UrlE = string.Empty;
                                string HeaderE = string.Empty;
                                string PriceE = string.Empty;
                                string RoomsE = string.Empty;
                                string AreaE = string.Empty;
                                string PricePerMeterE = string.Empty;

                                if (Element.IsPresent(offer, By.TagName("table")))
                                {
                                    var table = offer.FindElement(By.TagName("table"));

                                    IdE = table.GetAttribute("data-id");

                                    if (Element.IsPresent(table, By.ClassName("linkWithHash")))
                                    {
                                        var link = table.FindElement(By.ClassName("linkWithHash"));

                                        UrlE = link.GetAttribute("href");
                                        HeaderE = UrlE.Replace("https://www.olx.pl/oferta/", string.Empty);
                                    }

                                    if (Element.IsPresent(table, By.ClassName("price")))
                                    {
                                        var price = table.FindElement(By.ClassName("price"));

                                        PriceE = price.Text;
                                    }
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
                                        SiteType = SiteType.Olx
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