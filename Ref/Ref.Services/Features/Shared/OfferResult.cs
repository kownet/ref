using Ref.Data.Models;
using System;

namespace Ref.Services.Features.Shared
{
    public class OfferResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteOfferId { get; set; }
        public SiteType SiteType { get; set; }
        public DealType DealType { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public int Price { get; set; }
        public int Area { get; set; }
        public int Rooms { get; set; }
        public int PricePerMeter { get; set; }
        public DateTime DateAdded { get; set; }
    }
}