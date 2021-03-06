﻿using Ref.Data.Models;

namespace Ref.Data.Components
{
    public class MailReportOffer
    {
        public string Header { get; set; }
        public string Url { get; set; }
        public int Price { get; set; }
        public int Area { get; set; }
        public SiteType Site { get; set; }
    }
}