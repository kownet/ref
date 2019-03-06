using Ref.Data.Models;
using System;
using System.Collections.Generic;

namespace Ref.Data.Repositories.Standalone
{
    public class Client
    {
        public Client()
        {
            Filters = new HashSet<SearchFilter>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool Notification { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsChecked { get; set; }

        public IEnumerable<SearchFilter> Filters { get; set; }
        public Sleep Sleep { get; set; }

        public bool IsWorkingTime =>
            Sleep.IsActive || (Sleep.SleepFrom.HasValue && Sleep.SleepTo.HasValue)
            ? (Sleep.SleepFrom.Value > DateTime.Now.TimeOfDay && Sleep.SleepTo.Value < DateTime.Now.TimeOfDay)
            : false;
    }
}