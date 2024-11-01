using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Campus_Events.Models
{
    public class Participant :Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("EventId")]
        public Event Event
        {
            get; set;
        }
      }
}
