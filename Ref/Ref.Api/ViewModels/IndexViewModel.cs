namespace Ref.Api.ViewModels
{
    public class IndexViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string RegisteredAt { get; set; }
        public bool IsActive { get; set; }
        public int FiltersNumber { get; set; }

        public string RegisteredAtFormatted => $"Data rejestracji: {RegisteredAt}";
        public int Active => IsActive ? 1 : 0;
        public string FiltersNumberFormatted => $"Wykupiona liczba filtrów: {FiltersNumber}";
    }
}