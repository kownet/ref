using Ref.Data.Models;
using Ref.Shared.Extensions;
using Ref.Shared.Utils;

namespace Ref.Data.Repositories.Standalone
{
    public class SearchFilter
    {
        public string Name { get; set; }
        public PropertyType Property { get; set; }
        public DealType Deal { get; set; }
        public string Location { get; set; }
        public int FlatAreaFrom { get; set; }
        public int FlatAreaTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public MarketType Market { get; set; }

        public string LocationRaw => Location.RemoveDiacritics();
        public string Description() =>
            $"{Property.GetDescription()} / {Deal.GetDescription()} / " +
            $"{Market.GetDescription()} / {Location} / " +
            $"{Labels.FilterDescPrice}: {PriceFrom} - {PriceTo} / " +
            $"{Labels.FilterDescArea}: {FlatAreaFrom} - {FlatAreaTo}.";
    }
}