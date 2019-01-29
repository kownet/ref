using OpenQA.Selenium;

namespace Ref.Sites.Helpers.Pagination
{
    public interface IPagination
    {
        int Get(IWebDriver driver, string additionalInfo = "");
    }
}