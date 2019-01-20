using Ref.Data.Models;
using System.Collections.Generic;

namespace Ref.Sites
{
    public interface ISite
    {
        IEnumerable<Ad> Search(string type, string deal, string location, int flatAreaFrom, int flatAreaTo);
    }
}