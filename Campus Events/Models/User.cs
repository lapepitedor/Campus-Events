using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Campus_Events.Models
{
    public class User:Entity
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EMail { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public string? PasswordHash { get; set; }       
        public bool MailAddressConfirmed { get; set; }
        public string? PasswordResetToken { get; set; }

        public List<Claim> ToClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, EMail),
                new Claim("ID", ID.ToString())
            };

            // Ajoute le rôle d'admin dans les revendications si applicable
            if (IsAdmin == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            return claims;
        }
        // Relation avec les événements inscrits
        public ICollection<UserEvent> ? UserEvents { get; set; }
    }
}
