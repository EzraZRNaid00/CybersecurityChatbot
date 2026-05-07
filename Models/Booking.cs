using System;

namespace EventEase1.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int EventId { get; set; }
        public int VenueId { get; set; }

        // Make navigation properties nullable with '?'
        public Event? Event { get; set; }
        public Venue? Venue { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }
    }
}