using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EventEase1.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public string? ImageUrl { get; set; }

        // For image upload - not stored in database
        [NotMapped]
        [Display(Name = "Venue Image")]
        public IFormFile? ImageUpload { get; set; }
    }
}