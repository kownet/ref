using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum ReasonType
    {
        [Description("Koniec okresu próbnego")]
        DemoExceeded = 1,

        [Description("Niewystarczająca liczba filtrów")]
        FilterExceeded = 2,

        [Description("Propozycja nowego miasta")]
        CityProposition = 3,

        [Description("Propozycja dodania dzielnic")]
        DistrictProposition = 4,

        [Description("Kontakt ze strony PewneMieszkanie.pl")]
        ContactFromPm = 5
    }
}