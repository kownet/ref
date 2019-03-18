namespace Ref.Data.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRaw { get; set; }
        public string GtCodeSale { get; set; }
        public string GtCodeRent { get; set; }

        public bool IsGumtreeAvailable
            => !string.IsNullOrWhiteSpace(GtCodeSale) && !string.IsNullOrWhiteSpace(GtCodeRent);

        public bool IsGumtreeAvailableForSale
            => !string.IsNullOrWhiteSpace(GtCodeSale);

        public bool IsGumtreeAvailableForRent
            => !string.IsNullOrWhiteSpace(GtCodeRent);
    }
}