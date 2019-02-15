using Ref.Shared.Extensions;
using Ref.Shared.Utils;

namespace Ref.Data.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public int UserId { get; set; }

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
        public string Description() =>
            $"{Type.GetDescription()} / {Deal.GetDescription()} / " +
            $"{Market.GetDescription()} / {Location} / " +
            $"{Labels.FilterDescPrice}: {PriceFrom} - {PriceTo} / " +
            $"{Labels.FilterDescArea}: {FlatAreaFrom} - {FlatAreaTo}.";
    }
}