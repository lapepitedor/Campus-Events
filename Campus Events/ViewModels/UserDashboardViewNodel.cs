using Campus_Events.Models;

namespace Campus_Events.ViewModels
{
    public class UserDashboardViewModel
    {
        public IEnumerable<Event> UserEvents { get; set; } // Événements auxquels l'utilisateur est inscrit
        public PagedResult<Event> AllEvents { get; set; }   // Tous les événements avec pagination
    }
}
