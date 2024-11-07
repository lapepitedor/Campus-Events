using System.ComponentModel.DataAnnotations;

namespace Campus_Events.Models
{
    public class Event :Entity
    {
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public EventType Type { get; set; }  
        public string? Organizer { get; set; }
        // Places disponibles calculées comme TotalSeats - RegisteredSeats
        public int AvailableSeats => TotalSeats - UserEvents.Count;
        public int TotalSeats { get; set; } // Nombre total de places
        public bool IsActive { get; set; } = true;
        public int RegisteredSeatsCount { get; set; }
        // Navigation vers UserEvent pour les inscriptions
        public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}
