using OpenQA.Selenium.Chrome;
using Ref.Data.Models;
using Ref.Shared.Providers;
using Ref.Sites.Helpers;
using System;
using System.Collections.Generic;

namespace Ref.Sites
{
    public class DomiportaSite : BaseSite, ISite
    {
        public DomiportaSite(IAppProvider appProvider)
            : base(appProvider)
        {
        }

        public IEnumerable<Ad> Search(IFilterProvider filterProvider)
        {
            var result = new List<Ad>();

            var searchQuery = new QueryStringBuilder(SiteType.DomiPorta, filterProvider).Get();

            using (var driver = new ChromeDriver(_service, _options, TimeSpan.FromSeconds(DriverTimeSpan)))
            {
                driver.Navigate().GoToUrl(searchQuery);
            }

            return result;
        }
    }
}