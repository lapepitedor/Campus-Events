using Campus_Events.Models;
using System.ComponentModel.DataAnnotations;

namespace Campus_Events.ViewModels
{
    public class CreateOrEditEventViewModel
    {
        public Guid ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string  Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Organizer is required.")]
        [StringLength(50, ErrorMessage = "Organizer cannot exceed 50 characters.")]
        public string  Organizer  { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required.")]
        public EventType Type { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats must be greater than 0.")]
        public int TotalSeats { get; set; }

    }
}
