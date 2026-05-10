using Application.Profiles.Entities;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Entities
{
    public class ActivityEntity
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
        public required string Category { get; set; }
        public bool IsCancelled { get; set; }
        public required string HostDisplayName { get; set; }
        public required string HostId { get; set; }
        public required string City { get; set; }
        public required string Venue { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection<UserProfile> Attendees { get; set; } = [];
    }
}
