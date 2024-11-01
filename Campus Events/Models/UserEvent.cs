using System.ComponentModel.DataAnnotations;

namespace Campus_Events.Models
{
    public class UserEvent
    {
        
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now; // Facultatif, pour enregistrer la date d'inscription
    }
}
