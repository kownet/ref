using System;

namespace Ref.Data.Models
{
    public class Sleep
    {
        public bool IsActive { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public TimeSpan? SleepFrom
            => string.IsNullOrWhiteSpace(From)
            ? (TimeSpan?)null
            : DateTime.ParseExact(From, "HH:mm", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;

        public TimeSpan? SleepTo
            => string.IsNullOrWhiteSpace(To)
            ? (TimeSpan?)null
            : DateTime.ParseExact(To, "HH:mm", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;
    }
}