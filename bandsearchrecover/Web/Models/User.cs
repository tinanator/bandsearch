using System;
using BandSearch.Web.Models;
using FluentValidation;

namespace BandSearch.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PhotoUrl { get; set; }
        public string? About { get; set; }
        public bool? IsLookingForBand { get; set; }
        public string? BandOpenPositionCriteriaInfo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Band> Bands { get; set; } = new List<Band>();
        public virtual ICollection<InstrumentLevel> InstrumentsLevel { get; set; } = new List<InstrumentLevel>();
    }
}
