using Ref.Shared.Extensions;
using Ref.Shared.Utils;
using System;

namespace Ref.Data.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Name { get; set; }
        public PropertyType Property { get; set; }
        public DealType Deal { get; set; }
        public int CityId { get; set; }
        [Obsolete]
        public string Location { get; set; }
        public int FlatAreaFrom { get; set; }
        public int FlatAreaTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public MarketType Market { get; set; }
        public NotificationType Notification { get; set; }
        public DateTime? LastCheckedAt { get; set; }

        public string LocationRaw => Location.RemoveDiacritics();
        public string Description() =>
            $"{Property.GetDescription()} / {Deal.GetDescription()} / " +
            $"{Market.GetDescription()} / {Location} / " +
            $"{Labels.FilterDescPrice}: {PriceFrom} - {PriceTo} / " +
            $"{Labels.FilterDescArea}: {FlatAreaFrom} - {FlatAreaTo}.";
    }
}