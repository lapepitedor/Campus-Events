using Campus_Events.Models;

namespace Campus_Events.Persistence
{
    public interface IEventRepository
    {
        PagedResult<Event> GetAll(EventFilter filter);
        Event GetSingle(Guid id);
        Event Add(Event entity);
        void Delete(Guid id);
        Event Update(Event entity);

        // Nouvelle méthode pour obtenir les événements auxquels un utilisateur est inscrit
        IEnumerable<Event> GetEventsForUser(Guid userId);

    }
}
