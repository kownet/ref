using System;

namespace Ref.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string Role { get; set; }
        public SubscriptionType Subscription { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Guid { get; set; }
    }
}