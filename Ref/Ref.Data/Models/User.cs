using System;
using System.Collections.Generic;

namespace Ref.Data.Models
{
    public class User
    {
        public User()
        {
            Filters = new HashSet<Filter>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Notification { get; set; }
        public bool IsActive { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string Role { get; set; }
        public SubscriptionType Subscription { get; set; }
        public DateTime RegisteredAt { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

        public Sleep Sleep { get; set; }

        public bool IsWorkingTime =>
            Sleep.IsActive || (Sleep.SleepFrom.HasValue && Sleep.SleepTo.HasValue)
            ? (Sleep.SleepFrom.Value > DateTime.Now.TimeOfDay && Sleep.SleepTo.Value < DateTime.Now.TimeOfDay)
            : false;

        public bool IsChecked { get; set; }
    }
}