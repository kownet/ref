﻿using System;

namespace Ref.Data.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string SiteOfferId { get; set; }
        public SiteType Site { get; set; }
        public DealType Deal { get; set; }
        public PropertyType? Property { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public int Price { get; set; }
        public int Area { get; set; }
        public int Rooms { get; set; }
        public int PricePerMeter { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsScrapped { get; set; }
        public int? Floor { get; set; }
        public string Content { get; set; }
        public string Abstract { get; set; }
        public int? DistrictId { get; set; }
        public bool IsBadlyScrapped { get; set; }
        public bool IsFromPrivate { get; set; }
        public bool IsFromAgency { get; set; }

        public bool IsValidToAdd() =>
            !string.IsNullOrWhiteSpace(SiteOfferId) && !string.IsNullOrWhiteSpace(Url) && Price > 0;

        public bool ToDelete { get; set; }
    }
}