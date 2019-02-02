using Ref.Data.Models;
using System.Collections.Generic;

namespace Ref.Sites
{
    public interface ISite
    {
        IEnumerable<Ad> Search(IEnumerable<Filter> filterProvider);
    }
}