using Ref.Data.Models;
using System;

namespace Ref.Data.Components
{
    public class UserSubscription
    {
        public bool IsUserActive { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public int? FlatAreaFrom { get; set; }
        public int? FlatAreaTo { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public int? PricePerMeterFrom { get; set; }
        public int? PricePerMeterTo { get; set; }
        public PropertyType Property { get; set; }
        public DealType Deal { get; set; }
        public MarketType Market { get; set; }
        public NotificationType Notification { get; set; }
        public DateTime? LastCheckedAt { get; set; }
        public string ShouldContain { get; set; }
        public string ShouldNotContain { get; set; }
        public SubscriptionType Subscription { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int? DistrictId { get; set; }

        public bool DemoPassed
            => (Subscription == SubscriptionType.Demo && (DateTime.Now - RegisteredAt).TotalHours > 24);
    }
}