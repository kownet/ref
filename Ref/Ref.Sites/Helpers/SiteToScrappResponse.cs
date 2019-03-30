using HtmlAgilityPack;

namespace Ref.Sites.Helpers
{
    public class SiteToScrappResponse
    {
        public HtmlNode HtmlNode { get; set; }
        public bool ExceptionAccured { get; set; }
        public string ExceptionMessage { get; set; }

        public bool Succeed
            => HtmlNode != null &&
            !ExceptionAccured &&
            string.IsNullOrWhiteSpace(ExceptionMessage);
    }
}