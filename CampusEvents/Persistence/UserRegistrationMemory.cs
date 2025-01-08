using Campus_Events.Models;

namespace Campus_Events.Persistence
{
    public class UserRegistration : IUserRegistration
    {
        private readonly IEventRepository _eventRepository;

        public UserRegistration(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public bool RegisterUserToEvent(Guid eventId, Guid userId)
        {
            var eventItem = _eventRepository.GetSingle(eventId);

            // Vérifie si l'événement existe, s'il reste des places et si l'utilisateur n'est pas déjà inscrit
            if (eventItem != null && eventItem.AvailableSeats > eventItem.UserEvents.Count &&
                !eventItem.UserEvents.Any(ue => ue.UserId == userId))
            {
                // Crée une nouvelle inscription pour cet utilisateur
                var userEvent = new UserEvent
                {
                    EventId = eventId,
                    UserId = userId,
                    RegistrationDate = DateTime.Now
                };

                eventItem.UserEvents.Add(userEvent);       // Ajoute l'inscription à l'événement
                _eventRepository.Update(eventItem);        // Met à jour l'événement dans le dépôt

                return true; // Inscription réussie
            }

            return false; // Inscription échouée (déjà inscrit ou pas de places disponibles)
        }

        public bool UnregisterUserFromEvent(Guid eventId, Guid userId)
        {
            var eventItem = _eventRepository.GetSingle(eventId);

            // Vérifie si l'événement existe et si l'utilisateur est inscrit
            if (eventItem != null)
            {
                var registration = eventItem.UserEvents.FirstOrDefault(ue => ue.UserId == userId);
                if (registration != null)
                {
                    eventItem.UserEvents.Remove(registration);  // Retire l'inscription de UserEvents
                    _eventRepository.Update(eventItem);         // Met à jour l'événement dans le dépôt

                    return true; // Désinscription réussie
                }
            }

            return false; // Désinscription échouée (utilisateur non inscrit ou événement inexistant)
        }

        public bool IsUserRegistered(Guid eventId, Guid userId)
        {
            var eventItem = _eventRepository.GetSingle(eventId);
            return eventItem != null && eventItem.UserEvents.Any(ue => ue.UserId == userId);
        }
    }
}
