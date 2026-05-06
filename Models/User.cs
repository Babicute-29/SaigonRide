namespace SaigonRide.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? Passport { get; set; }
    }
}