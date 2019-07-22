namespace Ref.Api.ViewModels
{
    public class ErrorIndexViewModel
    {
        public bool DemoPassed { get; set; }
        public string Message { get; set; }

        public string FormattedHeader
            => DemoPassed
            ? "Koniec okresu próbnego"
            : "Ważna informacja";
    }
}