using Ref.Data.Models;
using System.Collections.Generic;

namespace Ref.Sites.Helpers
{
    public class ScrappResponse
    {
        public ScrappResponse()
        {
            Offers = new HashSet<Offer>();
        }

        public IEnumerable<Offer> Offers { get; set; }

        public bool WeAreBanned { get; set; }
        public bool ThereAreNoRecords { get; set; }
    }
}