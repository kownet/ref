using OpenQA.Selenium;
using Ref.Shared.Extensions;

namespace Ref.Sites.Helpers.Pagination
{
    public class DomiportaPagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
            int pages = 1;

            if (Element.IsPresent(driver, By.ClassName("pagination")))
            {
                var pagesElementText = driver.FindElement(By.ClassName("pagination"));

                if (Element.IsPresent(pagesElementText, By.TagName("li")))
                {
                    var pagesElements = pagesElementText.FindElements(By.TagName("li"));

                    if (pagesElements.AnyAndNotNull())
                    {
                        if (int.TryParse(pagesElements.SecondLast().Text, out pages))
                        {

                        }
                    }
                }
            }

            return pages;
        }
    }
}