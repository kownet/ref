namespace Ref.Api.ViewModels
{
    public class FilterViewModel
    {
        public string Name { get; set; }
        public int Property { get; set; }
        public int City { get; set; }
        public int? District { get; set; }
        public int Notification { get; set; }
        public int? AreaFrom { get; set; }
        public int? AreaTo { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public int? PpmFrom { get; set; }
        public int? PpmTo { get; set; }
        public string ShouldContain { get; set; }
        public string ShouldNotContain { get; set; }
        public string UserGuid { get; set; }
        public int UserId { get; set; }
        public int FilterId { get; set; }
        public int CityHasDistricts { get; set; }
        public int AllowFromAgency { get; set; }
    }
}