namespace Ref.Api.ViewModels
{
    public class ErrorContactViewModel
    {
        public string Message { get; set; }
        public bool IsException { get; set; }
        public string UserGuid { get; set; }
        public string FormattedHeader
            => IsException
            ? "Błąd"
            : "Ważna informacja";
    }
}