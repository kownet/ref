namespace Ref.Data.Models
{
    public class District
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string NameRaw { get; set; }
        public string GtCodeSale { get; set; }
        public string GtCodeRent { get; set; }
        public int? OtoDomId { get; set; }
        public int? OlxId { get; set; }

        public bool IsGumtreeAvailable
            => !string.IsNullOrWhiteSpace(GtCodeSale) && !string.IsNullOrWhiteSpace(GtCodeRent);

        public bool IsGumtreeAvailableForSale
            => !string.IsNullOrWhiteSpace(GtCodeSale);

        public bool IsGumtreeAvailableForRent
            => !string.IsNullOrWhiteSpace(GtCodeRent);

        public bool IsOtoDomAvailable
            => OtoDomId.HasValue;

        public bool IsOlxAvailable
            => OlxId.HasValue;
    }
}