namespace Ref.Services.Features.Shared
{
    public class CityResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRaw { get; set; }
        public bool HasDistricts { get; set; }

        public string NameFormatted => HasDistricts ? $"{Name} (dzielnice)" : Name;
    }
}