using Ref.Data.Models;
using Ref.Sites.Helpers;
using System.Collections.Generic;

namespace Ref.Sites
{
    public interface ISite
    {
        SiteResponse Search(IEnumerable<Filter> filterProvider);
    }
}