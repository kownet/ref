using System.ComponentModel;

namespace Ref.Data.Models
{
    public enum NotificationType
    {
        [Description("Natychmiast")]
        Immediately = 0,

        [Description("Co godzinę")]
        EveryHour = 1,

        [Description("Co 2 godziny")]
        Every2Hour = 2,

        [Description("Co 4 godziny")]
        Every4Hour = 4,

        [Description("Co 6 godzin")]
        Every6Hour = 6,

        [Description("Co 8 godzin")]
        Every8Hour = 8,

        [Description("Nie aktywny")]
        NotActive = 100,
    }
}