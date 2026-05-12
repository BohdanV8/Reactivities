using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class UserFollowing
    {
        public string ObserverId { get; set; }
        public User Observer { get; set; }
        public string TargetId { get; set; }
        public User Target { get; set; }
    }
}
