using Ref.Data.Models;

namespace Ref.Services.Features.Shared
{
    public class FilterResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public int FlatAreaFrom { get; set; }
        public int FlatAreaTo { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public NotificationType Notification { get; set; }
    }
}