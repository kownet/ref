using System;

namespace Ref.Data.Models
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
    }
}