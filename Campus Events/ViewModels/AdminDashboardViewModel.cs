using Campus_Events.Models;

namespace Campus_Events.ViewModels
{
    public class AdminDashboardViewModel
    {
        public PagedResult<Event>? Events { get; set; }
        public string ? SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
