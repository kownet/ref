﻿namespace Ref.Api.ViewModels
{
    public class ErrorAddViewModel
    {
        public string Message { get; set; }
        public bool IsDemo { get; set; }
        public bool IsException { get; set; }
        public string UserGuid { get; set; }
        public bool IsTooMuch { get; set; }
        public string FormattedHeader
            => IsException
            ? "Błąd"
            : "Ważna informacja";
    }
}