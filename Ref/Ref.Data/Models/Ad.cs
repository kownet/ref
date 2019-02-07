namespace Ref.Data.Models
{
    public class Ad
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string Price { get; set; }
        public string Rooms { get; set; }
        public string Area { get; set; }
        public string PricePerMeter { get; set; }
        public SiteType SiteType { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Url);
    }
}