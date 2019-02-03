﻿using OpenQA.Selenium;
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
    public class GratkaSite : BaseSite, ISite
    {
        public GratkaSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public SiteResponse Search(IEnumerable<Filter> filterProvider)
        {
            var filter = filterProvider.First();

            var result = new List<Ad>();

            var searchQuery = new GratkaQueryString(filter).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                int pages = new GratkaPagination().Get(driver);

                for (int i = 1; i <= pages; i++)
                {
                    driver.Navigate().GoToUrl($@"{searchQuery}&page={i}");

                    if (Element.IsPresent(driver, By.ClassName("content__listing")))
                    {
                        var listing = driver.FindElement(By.ClassName("content__listing"));

                        if (Element.IsPresent(listing, By.TagName("article")))
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
            return new SiteResponse
            {
                FilterName = filter.Name,
                Advertisements = result
            };
        }
    }
}