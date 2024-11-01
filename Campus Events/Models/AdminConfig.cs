namespace Campus_Events.Models
{
    public static class AdminConfig
    {
        public static readonly List<(string Email, string Password)> Admins = new List<(string Email, string Password)>
    {
        ("admin@yahoo.com", "secretsecret"),
        ("admin@gmail.com", "secretsecret")
    };
    }
}
