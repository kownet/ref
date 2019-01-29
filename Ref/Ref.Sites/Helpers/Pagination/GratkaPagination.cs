using OpenQA.Selenium;

namespace Ref.Sites.Helpers.Pagination
{
    public class GratkaPagination : IPagination
    {
        public int Get(IWebDriver driver, string additionalInfo = "")
        {
            int pages = 1;

            if (Element.IsPresent(driver, By.ClassName("pagination")))
            {
                var pagesElementText = driver.FindElement(By.ClassName("pagination"));

                if (Element.IsPresent(pagesElementText, By.TagName("input")))
                {
                    var input = pagesElementText.FindElement(By.TagName("input"));

                    if (int.TryParse(input.GetAttribute("max"), out pages))
                    {

                    }
                }
            }

            return pages;
        }
    }
}