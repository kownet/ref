using Ref.Data.Repositories.Standalone;
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
        public bool WeAreBanned { get; set; }
        public bool ExceptionAccured { get; set; }
        public string ExceptionMessage { get; set; }
    }
}