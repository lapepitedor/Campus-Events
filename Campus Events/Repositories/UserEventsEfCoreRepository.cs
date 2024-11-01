using Campus_Events.Models;
using Campus_Events.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campus_Events.Repositories
{
    public class UserEventsEfCoreRepository : IUserRegistration
    {
        private readonly ApplicationDbContext context;
        public UserEventsEfCoreRepository(ApplicationDbContext context) 
        {
            this.context = context;
            context.Migrate();
        } 
        public bool IsUserRegistered(Guid eventId, Guid userId)
        {
            return context.UserEvents.Any(ue => ue.EventId == eventId && ue.UserId == userId);
        }

        public bool RegisterUserToEvent(Guid eventId, Guid userId)
        {
            var eventItem = context.Events.Find(eventId);
            var user = context.Users.Find(userId);

            if (eventItem == null || user == null || eventItem.AvailableSeats <= 0)
            {
                return false; // Soit l'événement, soit l'utilisateur n'existe pas, soit plus de places disponibles
            }

            // Vérifie si l'utilisateur est déjà inscrit
            bool isAlreadyRegistered = context.UserEvents.Any(ue => ue.EventId == eventId && ue.UserId == userId);

            if (isAlreadyRegistered)
            {
                return false; // L'utilisateur est déjà inscrit
            }

            // Crée une nouvelle inscription
            var userEvent = new UserEvent
            {
                EventId = eventId,
                UserId = userId,
                RegistrationDate = DateTime.Now
            };

            context.UserEvents.Add(userEvent);
            context.SaveChanges(); // Persiste les modifications

            return true; // Inscription réussie
        }

        public bool UnregisterUserFromEvent(Guid eventId, Guid userId)
        {
            // Trouve l'inscription
            var registration = context.UserEvents
                .FirstOrDefault(ue => ue.EventId == eventId && ue.UserId == userId);

            if (registration == null)
            {
                return false; // L'utilisateur n'est pas inscrit à cet événement
            }

            context.UserEvents.Remove(registration);
            context.SaveChanges(); // Persiste la désinscription

            return true; // Désinscription réussie
        }
    }
}
