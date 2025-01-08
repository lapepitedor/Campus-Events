using System.ComponentModel.DataAnnotations;

namespace Campus_Events.ViewModels
{
    public class PasswordForgottenViewModel
    {
        [Required]
        [EmailAddress]
        public string? EMail { get; set; }
    }
}
