namespace SaigonRide.Models
{
    public class User
    {
        public int Id { get; set; }

        // Gán = string.Empty để tránh lỗi Nullable warning
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? Passport { get; set; }

        // FIX LỖI: Thêm thuộc tính Role để phân biệt Admin và User
        public string Role { get; set; } = "User";
    }
}