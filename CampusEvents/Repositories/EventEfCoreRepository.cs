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
            var query = context.Events.AsNoTracking();

            if (filter.FilterExpressions != null)
            {
                foreach (var expression in filter.FilterExpressions)
                {
                    if (!string.IsNullOrEmpty(expression.PropertyName) && !string.IsNullOrEmpty(expression.Value))
                    {
                        switch (expression.Relation)
                        {
                            case RelationType.Equal:
                                query = query.Where(e => EF.Property<string>(e, expression.PropertyName) == expression.Value);
                                break;
                            case RelationType.NotEqual:
                                query = query.Where(e => EF.Property<string>(e, expression.PropertyName) != expression.Value);
                                break;
                            case RelationType.Larger:
                                query = query.Where(e => EF.Property<DateTime>(e, expression.PropertyName) > DateTime.Parse(expression.Value));
                                break;
                            case RelationType.Smaller:
                                query = query.Where(e => EF.Property<DateTime>(e, expression.PropertyName) < DateTime.Parse(expression.Value));
                                break;
                        }
                    }
                }
            }


            // Pagination parameter validation
            int startPage = filter.StartPage < 1 ? 1 : filter.StartPage; // Ensures that StartPage is always >= 1
            var currentPage = filter.StartPage > 0 ? filter.StartPage : 1;

            // Calculate the pagination
            int totalCount = query.Count();
            var items = query
                .Skip((startPage - 1) * filter.ItemsPerPage)
                .Take(filter.ItemsPerPage)
                .ToList();


            return new PagedResult<Event>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = filter.ItemsPerPage,
                CurrentPage = currentPage
            };
        }

        public IEnumerable<Event> GetEventsForUser(Guid userId)
        {
            var ret = context.UserEvents
                 .Where(ue => ue.UserId == userId)
                 .Select(ue => ue.Event)
                 .ToList();

            return ret;
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
