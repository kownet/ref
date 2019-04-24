using System;

namespace Ref.Data.Models
{
    public class Event
    {
        public int Id { get; set; }
        public EventCategory Category { get; set; }
        public EventType Type { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; }
        public string City { get; set; }
        public string District { get; set; }
    }
}