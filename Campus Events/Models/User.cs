using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Campus_Events.Models
{
    public class User:Entity
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EMail { get; set; }
        public string? PasswordHash { get; set; }
        public bool? IsAdmin { get; set; }
        public bool MailAddressConfirmed { get; set; }
        public string? PasswordResetToken { get; set; }

        public List<Claim> ToClaims()
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, EMail),
            new Claim("ID", ID.ToString())
        };

            //if (IsAdmin)
            //    claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            return claims;
        }
        // Relation avec les événements inscrits
        public ICollection<UserEvent> UserEvents { get; set; }
    }
}
