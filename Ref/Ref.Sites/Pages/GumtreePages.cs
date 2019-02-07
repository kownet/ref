using HtmlAgilityPack;
using Ref.Shared.Extensions;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class GumtreePages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".pagination").FirstOrDefault();

            if (!(pager is null))
            {
                var after = pager.CssSelect(".after").FirstOrDefault();

                if (!(after is null))
                {
                    var a = after.CssSelect("a");

                    if (!(a is null))
                    {
                        if (a.AnyAndNotNull())
                        {
                            var lastLink = a.Last().ByAttribute("href");

                            var lastPageValue = lastLink.TextAfter(additionalInfo).First().ToString();

                            int.TryParse(lastPageValue, out pages);
                        }
                    }
                }
            }
            return pages;
        }
    }
}