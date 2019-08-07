namespace Ref.Sites.Helpers
{
    public class SingleScrappResponse
    {
        private static readonly int _maxLenght = 50;

        public int? Floor { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRedirected { get; set; }

        public int? PricePerMeter { get; set; }
        public int? Rooms { get; set; }
        public int? Area { get; set; }

        public bool IsFromPrivate { get; set; }
        public bool IsFromAgency { get; set; }

        public bool ParamsAreValid
            => (Area.HasValue && Area.Value != 0) && (PricePerMeter.HasValue && PricePerMeter.Value != 0);

        public bool Succeed
            => !string.IsNullOrWhiteSpace(Content) && !IsDeleted && !IsRedirected && ParamsAreValid;

        public string Abstract
            => $"{Content.PadRight(_maxLenght).Substring(0, _maxLenght).TrimEnd().ToLowerInvariant()}";
    }
}