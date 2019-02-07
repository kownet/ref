using HtmlAgilityPack;

namespace Ref.Sites.Pages
{
    public interface IPages
    {
        int Get(HtmlNode html, string additionalInfo = "");
    }
}