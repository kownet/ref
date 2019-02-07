using HtmlAgilityPack;
using Ref.Sites.Helpers;
using ScrapySharp.Extensions;
using System.Linq;

namespace Ref.Sites.Pages
{
    public class GratkaPages : IPages
    {
        public int Get(HtmlNode html, string additionalInfo = "")
        {
            int pages = 1;

            var pager = html.CssSelect(".pagination").FirstOrDefault();

            if (!(pager is null))
            {
                var current = pager.CssSelect("input").FirstOrDefault();

                if (!(current is null))
                {
                    var max = current.ByAttribute("max");

                    int.TryParse(max, out pages);
                }
            }
            return pages;
        }
    }
}