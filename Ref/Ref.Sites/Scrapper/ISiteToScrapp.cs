using Ref.Data.Models;
using Ref.Sites.Helpers;
using System.Collections.Generic;

namespace Ref.Sites.Scrapper
{
    public interface ISiteToScrapp
    {
        SiteResponse Search(IEnumerable<Filter> filterProvider);
    }
}