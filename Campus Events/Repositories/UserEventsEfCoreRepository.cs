using Campus_Events.Models;
using Campus_Events.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campus_Events.Repositories
{
    public class UserEventsEfCoreRepository : IUserRegistration
    {
        private readonly ApplicationDbContext _context;

        public UserEventsEfCoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsUserRegistered(Guid eventId, Guid userId)
        {
            return _context.UserEvents.Any(ue => ue.EventId == eventId && ue.UserId == userId);
        }

        public bool RegisterUserToEvent(Guid eventId, Guid userId)
        {
            var eventItem = _context.Events
                .Include(e => e.UserEvents)
                .FirstOrDefault(e => e.ID == eventId);

            var user = _context.Users.Find(userId);

            if (eventItem == null || user == null || eventItem.AvailableSeats <= 0)
            {
                return false; // No event or no user or no places available
            }

            // Check if the user is already registered
            if (IsUserRegistered(eventId, userId))
            {
                return false; 
            }

            // Create a registration
            var userEvent = new UserEvent
            {
                EventId = eventId,
                UserId = userId,
                RegistrationDate = DateTime.Now
            };

            _context.UserEvents.Add(userEvent);
            eventItem.RegisteredSeatsCount++;

            try
            {
                _context.SaveChanges(); 
            }
            catch (DbUpdateException ex)
            {
                return false; 
            }

            return true; //  Successful registration
        }


        public bool UnregisterUserFromEvent(Guid eventId, Guid userId)
        {
            var registration = _context.UserEvents
                .FirstOrDefault(ue => ue.EventId == eventId && ue.UserId == userId);

            if (registration == null)
            {
                return false;
            }

            _context.UserEvents.Remove(registration);
            var eventItem = _context.Events.Find(eventId);
            if (eventItem != null)
            {
                eventItem.RegisteredSeatsCount--;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                
                return false;
            }

            return true;
        }
    }
}
