using Campus_Events.Models;

namespace Campus_Events.Persistence
{
    public class EventMemoryRepository :IEventRepository
    {
        protected Dictionary<Guid, Event> items = new Dictionary<Guid, Event>();
        public EventMemoryRepository()
        {
            Add(new Event() { Title = "Learn ASP.net Core", TotalSeats=100 });
            Add(new Event() { Title = "Learn italian", TotalSeats= 40, Location="Ham 5" });
        }

        public IEnumerable<Event> GetAll()
        {
            return items.Values;
        }

        private IEnumerable<Event> Filter(IEnumerable<Event> input, EventFilter filter)
        {
            if (filter is null || filter.FilterExpressions is null || filter.FilterExpressions.Count() == 0)
                return input;

            var result = new List<Event>();
            foreach (var item in input)
            {
                bool applies = true;
                foreach (var expr in filter.FilterExpressions)
                    applies = applies && CompliesFilter(item, expr);

                if (applies)
                    result.Add(item);
            }

            return result;
        }

        private IEnumerable<Event> Order(IEnumerable<Event> input, EventFilter filter)
        {
            if (filter is null || string.IsNullOrEmpty(filter.OrderBy) || input.Count() == 0)
                return input;

            var prop = input.First().GetType().GetProperty(filter.OrderBy);
            return input.OrderBy(x => prop.GetValue(x, null));
        }

        private IEnumerable<Event> Paginate(IEnumerable<Event> input, EventFilter filter)
        {
            if (filter is null || filter.StartPage == -1)
                return input;

            return input.Skip((filter.StartPage - 1) * filter.ItemsPerPage).Take(filter.ItemsPerPage);
        }

        //public PagedResult<Event> GetAll(EventFilter filter)
        //{
        //    if (filter is null)
        //        filter = new EventFilter();

        //    var objects = Filter(items.Values, filter);
        //    var result = new PagedResult<Event>()
        //    {
        //        CurrentPage = filter.StartPage >= 0 ? filter.StartPage : 0,
        //        TotalItems = objects.Count(),
        //        TotalPages = (objects.Count() / filter.ItemsPerPage) + 1
        //    };

        //    objects = Order(objects, filter);
        //    result.Items = Paginate(objects, filter);
        //    return result;
        //}

        private bool CompliesFilter(object obj, FilterExpression expression)
        {
            var prop = obj.GetType().GetProperty(expression.PropertyName);


            dynamic value = prop.GetValue(obj, null);
            dynamic comparerValue = Convert.ChangeType(expression.Value, prop.PropertyType);

            switch (expression.Relation)
            {
                case RelationType.Equal: return value == comparerValue;
                case RelationType.NotEqual: return value != comparerValue;
                case RelationType.Larger: return value > comparerValue;
                case RelationType.LargerOrEqual: return value >= comparerValue;
                case RelationType.Smaller: return value < comparerValue;
                case RelationType.SmallerOrEqual: return value <= comparerValue;
            }

            return false;
        }

        private IEnumerable<object> OrderBy(IEnumerable<object> list, string properytName)
        {
            var prop = list.First().GetType().GetProperty(properytName);
            return list.OrderBy(x => prop?.GetValue(x, null));
        }

        public Event GetSingle(Guid id)
        {
            if (items.ContainsKey(id))
                return items[id];

            return null;
        }

        public Event Add(Event entity)
        {
            entity.ID = Guid.NewGuid();
            items.Add(entity.ID, entity);
            return entity;
        }

        public Event Update(Event entity)
        {
            items[entity.ID] = entity;
            return entity;
        }

        public void Delete(Guid id)
        {
            items.Remove(id);
        }

        public IEnumerable<Event> GetEventsForUser(Guid userId)
        {
            return items.Values.Where(e => e.UserEvents.Any(ue => ue.UserId == userId));
        }

        public PagedResult<Event> GetAll(EventFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
