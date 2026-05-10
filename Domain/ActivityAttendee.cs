using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ActivityAttendee
    {
        public string? UserId { get; set; }
        public User User { get; set; }
        public Guid? ActivityId { get; set; }
        public Activity Activity { get; set; }
        public bool IsHost { get; set; }
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}
