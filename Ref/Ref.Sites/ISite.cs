using Ref.Data.Models;
using Ref.Shared.Providers;
using System.Collections.Generic;

namespace Ref.Sites
{
    public interface ISite
    {
        IEnumerable<Ad> Search(IFilterProvider filterProvider);
    }
}