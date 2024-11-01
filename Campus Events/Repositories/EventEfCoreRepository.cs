using Campus_Events.Models;
using Campus_Events.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campus_Events.Repositories
{
    public class EventEfCoreRepository : IEventRepository
    {
        private readonly ApplicationDbContext context;

        public EventEfCoreRepository(ApplicationDbContext context)
        {
            this.context = context;
            context.Migrate();
        }

        public Event Add(Event entity)
        {
            context.Events.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Delete(Guid id)
        {
            var entity = context.Events.Find(id);
            if (entity != null)
            {
                context.Events.Remove(entity);
                context.SaveChanges();
            }
        }

        public PagedResult<Event> GetAll(EventFilter filter)
        {
            var query = context.Events.AsQueryable();

            var totalCount = query.Count();
            IEnumerable<Event> result;

            if (filter.StartPage >= 0)
                result = query.AsNoTracking()
                    .Skip(filter.StartPage * filter.ItemsPerPage)
                    .Take(filter.ItemsPerPage)
                    .ToList();
            else
                result = query.AsNoTracking().ToList();

            return new PagedResult<Event>
            {
                TotalItems = totalCount,
                CurrentPage = filter.StartPage,
                TotalPages = totalCount / filter.ItemsPerPage + 1,
                Items = result
            };
        }

        public IEnumerable<Event> GetEventsForUser(Guid userId)
        {
            return context.UserEvents
                 .Where(ue => ue.UserId == userId)
                 .Select(ue => ue.Event)
                 .ToList();
        }

        public Event GetSingle(Guid id)
        {
            return context.Events.AsNoTracking().Where(x => x.ID == id).FirstOrDefault();
        }

        public Event Update(Event entity)
        {
            context.Events.Update(entity);
            context.SaveChanges();
            context.Entry(entity).State = EntityState.Detached;
            return entity;
        }
    }
}
