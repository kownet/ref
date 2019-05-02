using Ref.Data.Models;

namespace Ref.Services.Features.Shared
{
    public class ToScrappResult
    {
        public int ToScrapp { get; set; }
        public SiteType Site { get; set; }
        public string Formatted => Site.ToString();
    }
}