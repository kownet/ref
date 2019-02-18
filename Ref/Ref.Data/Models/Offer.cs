using System;

namespace Ref.Data.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string SiteOfferId { get; set; }
        public SiteType SiteType { get; set; }
        public DealType DealType { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public int Price { get; set; }
        public DateTime DateAdded { get; set; }
    }
}