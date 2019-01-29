using OpenQA.Selenium;
using Ref.Shared.Extensions;

namespace Ref.Sites.Helpers.Pagination
{
    public class OlxPagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
            int pages = 1;

            if (Element.IsPresent(driver, By.ClassName("pager")))
            {
                var pagesElementText = driver.FindElement(By.ClassName("pager"));

                if (Element.IsPresent(pagesElementText, By.TagName("a")))
                {
                    var elements = pagesElementText.FindElements(By.TagName("a"));

                    if (elements.AnyAndNotNull())
                    {
                        var last = elements.SecondLast();

                        if (int.TryParse(last.Text, out pages))
                        {

                        }
                    }
                }
            }

            return pages;
        }
    }
}