using OpenQA.Selenium;
using Ref.Shared.Extensions;
using System.Linq;

namespace Ref.Sites.Helpers.Pagination
{
    public class AdresowoPagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
            int pages = 1;

            if (Element.IsPresent(driver, By.ClassName("content-indent")))
            {
                var pagesElementText = driver.FindElement(By.ClassName("content-indent"));

                if (Element.IsPresent(pagesElementText, By.ClassName("pure-button")))
                {
                    var pagesElement = pagesElementText.FindElements(By.ClassName("pure-button"));

                    if (pagesElement.AnyAndNotNull())
                    {
                        var last = pagesElement.Last().Text;

                        if (int.TryParse(last, out pages))
                        {

                        }
                    }
                }
            }

            return pages;
        }
    }
}