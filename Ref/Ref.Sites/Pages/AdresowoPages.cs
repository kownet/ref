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

            var pager = html.CssSelect(".content-indent").FirstOrDefault();

            if (!(pager is null))
            {
                var elements = pager.CssSelect(".pure-button");

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