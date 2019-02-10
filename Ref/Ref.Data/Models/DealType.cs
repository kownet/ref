using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum DealType
    {
        [Description("Sprzedaż")]
        Sale = 0,

        [Description("Wynajem")]
        Rent = 1
    }
}