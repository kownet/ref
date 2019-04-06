namespace Ref.Sites.Helpers
{
    public class SingleScrappResponse
    {
        public int? Floor { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRedirected { get; set; }

        public int? PricePerMeter { get; set; }
        public int? Rooms { get; set; }
        public int? Area { get; set; }

        public bool Succeed => !string.IsNullOrWhiteSpace(Content) && !IsDeleted && !IsRedirected;
    }
}