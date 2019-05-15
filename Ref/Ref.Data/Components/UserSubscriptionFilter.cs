using Ref.Data.Models;

namespace Ref.Data.Components
{
    public class UserSubscriptionFilter
    {
        public int CityId { get; set; }
        public string City { get; set; }
        public string GumtreeCitySale { get; set; }
        public int? FlatAreaFrom { get; set; }
        public int? FlatAreaTo { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public PropertyType Property { get; set; }
        public DealType Deal { get; set; }
        public MarketType Market { get; set; }
        public int? DistrictId { get; set; }
        public string District { get; set; }
        public string GumtreeDistrictSale { get; set; }
        public int? OlxId { get; set; }
        public string AdrId { get; set; }
    }
}