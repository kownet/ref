using OpenQA.Selenium;

namespace Ref.Sites.Helpers.Pagination
{
    public class OtoDomPagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
            int pages = 1;

            if (Element.IsPresent(driver, By.ClassName("pager")))
            {
                var pagesElementText = driver.FindElement(By.ClassName("pager"));

                if (Element.IsPresent(pagesElementText, By.ClassName("current")))
                {
                    var current = driver.FindElement(By.ClassName("current")).Text;

                    if (int.TryParse(current, out pages))
                    {

                    }
                }
            }

            return pages;
        }
    }
}