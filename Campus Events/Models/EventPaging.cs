namespace Campus_Events.Models
{
    public class EventPaging
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; } //total record items
        public int PageSize {get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public EventPaging()
        {

        }

        public EventPaging(int totalItems, int page, int pageSize =10)
        {
            int totalPages = (int)Math.Ceiling((double)totalItems / (double)pageSize);
            int currentPage = page;

            int startPage = currentPage - 1;
            int endPage = currentPage + 3;


            if (startPage <= 0) {
            endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage =totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            StartPage = startPage;
            EndPage = endPage;
        }
    }

    
}
