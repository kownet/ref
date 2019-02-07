using HtmlAgilityPack;
using Ref.Shared.Extensions;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class OlxPages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".pager").FirstOrDefault();

            if (!(pager is null))
            {
                var elements = pager.CssSelect("a");

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