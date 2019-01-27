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
    public class OlxSite : BaseSite, ISite
    {
        public OlxSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.Olx, filterProvider).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);

                if (Element.IsPresent(driver, By.XPath("//*[@id=\"offers_table\"]")))
                {
                    var listing = driver.FindElement(By.XPath("//*[@id=\"offers_table\"]"));

                    var offers = listing.FindElements(By.ClassName("offer"));

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

                            if (Element.IsPresent(offer, By.XPath("//*[@id=\"offers_table\"]/tbody/tr[5]/td/div/table")))
                                IdE = offer.FindElement(By.XPath("//*[@id=\"offers_table\"]/tbody/tr[5]/td/div/table")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-price")))
                                UrlE = offer.FindElement(By.ClassName("offer-item-price")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-title")))
                                HeaderE = offer.FindElement(By.ClassName("offer-item-title")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-price")))
                                PriceE = offer.FindElement(By.ClassName("offer-item-price")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-rooms")))
                                RoomsE = offer.FindElement(By.ClassName("offer-item-rooms")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-area")))
                                AreaE = offer.FindElement(By.ClassName("offer-item-area")).Text;

                            if (Element.IsPresent(offer, By.ClassName("offer-item-price-per-m")))
                                PricePerMeterE = offer.FindElement(By.ClassName("offer-item-price-per-m")).Text;
                        }
                    }
                }
            }

            return result;
        }
    }
}