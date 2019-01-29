using OpenQA.Selenium;
using Ref.Shared.Extensions;
using System.Linq;

namespace Ref.Sites.Helpers.Pagination
{
    public class GumtreePagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
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

                            var lastPageValue = lastPageLink.TextAfter(additionalInfo).First().ToString();

                            if (int.TryParse(lastPageValue, out pages))
                            {

                            }
                        }
                    }
                }
            }

            return pages;
        }
    }
}