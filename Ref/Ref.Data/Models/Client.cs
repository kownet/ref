using System.Collections.Generic;

namespace Ref.Data.Models
{
    public class Client
    {
        public Client()
        {
            Filters = new HashSet<Filter>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public IEnumerable<Filter> Filters { get; set; }
    }
}