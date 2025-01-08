using System.ComponentModel.DataAnnotations;

namespace Campus_Events.ViewModels
{
    public class ForgottenPasswordVM
    {

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public bool EmailSent {  get; set; }

    }
}
