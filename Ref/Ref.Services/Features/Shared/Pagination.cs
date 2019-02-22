using System;

namespace Ref.Services.Features.Shared
{
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalRecords { get; set; }

        public int? TotalPages => TotalRecords.HasValue 
            ? (int)Math.Ceiling(TotalRecords.Value / (double)PageSize)
            : (int?)null;
    }
}