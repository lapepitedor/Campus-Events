using System.ComponentModel.DataAnnotations;

namespace Campus_Events.ViewModels
{
    public class PasswordResetViewModel:IValidatableObject
    {
        public string? Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? PasswordRepeat { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Password.Equals(PasswordRepeat))
                yield return new ValidationResult("Passwords are not equal!", new List<string> { "PasswordRepeat" });
        }
    }
}
