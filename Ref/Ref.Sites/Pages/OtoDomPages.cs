using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class OtoDomPages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".pager").FirstOrDefault();

            if (!(pager is null))
            {
                var current = pager.CssSelect(".current").FirstOrDefault();

                if (!(current is null))
                {
                    int.TryParse(current.InnerText, out pages);
                }
            }
            return pages;
        }
    }
}