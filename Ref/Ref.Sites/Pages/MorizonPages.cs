using HtmlAgilityPack;
using Ref.Shared.Extensions;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class MorizonPages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".mz-pagination-number").FirstOrDefault();

            if (!(pager is null))
            {
                var elements = pager.CssSelect("li");

                if (!(elements is null))
                {
                    if (elements.AnyAndNotNull())
                    {
                        var last = elements.SecondLast();

                        int.TryParse(last.InnerText, out pages);
                    }
                }
            }
            return pages;
        }
    }
}