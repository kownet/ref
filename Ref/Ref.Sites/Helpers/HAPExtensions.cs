using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ref.Sites.Helpers
{
    public static class HAPExtensions
    {
        public static string ByClass(this HtmlNode node, string classId, string regex = @"[^a-zA-Z0-9łŁęĘąĄćĆŹóÓńŃśŚ ,.-]")
        {
            var header = node.CssSelect($".{classId}").FirstOrDefault();

            if (!(header is null))
            {
                if (!string.IsNullOrWhiteSpace(header.InnerText))
                {
                    var txt = header.InnerText.Replace("&#160;", " ");
                    return Regex.Replace(txt, regex, string.Empty).Trim();
                }
                else return string.Empty;
            }
            else
                return string.Empty;
        }

        public static string ByAttribute(this HtmlNode node, string attributeId)
            => node.Attributes[attributeId] != null
                ? node.Attributes[attributeId].Value
                : string.Empty;
    }
}