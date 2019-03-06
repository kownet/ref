using HtmlAgilityPack;
using Ref.Shared.Extensions;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class AdresowoPages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".search-pagination").FirstOrDefault();

            if (!(pager is null))
            {
                var elements = pager.CssSelect(".search-pagination__number");

                if (!(elements is null))
                {
                    if (elements.AnyAndNotNull())
                    {
                        var last = elements.Last();

                        int.TryParse(last.InnerText, out pages);
                    }
                }
            }
            return pages;
        }
    }
}