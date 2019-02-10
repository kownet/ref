using Ref.Shared.Extensions;

namespace Ref.Data.Models
{
    public class Filter
    {
        public string Name { get; set; }
        public PropertyType Type { get; set; }
        public DealType Deal { get; set; }
        public string Location { get; set; }
        public int FlatAreaFrom { get; set; }
        public int FlatAreaTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public MarketType Market { get; set; }
        public int Newest { get; set; }

        public string LocationRaw => Location.RemoveDiacritics();
    }
}