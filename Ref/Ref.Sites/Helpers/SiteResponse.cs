using Ref.Data.Models;
using System.Collections.Generic;

namespace Ref.Sites.Helpers
{
    public class SiteResponse
    {
        public SiteResponse()
        {
            Advertisements = new HashSet<Ad>();
        }

        public IEnumerable<Ad> Advertisements { get; set; }
        public string FilterName { get; set; }
        public string FilterDesc { get; set; }
    }
}