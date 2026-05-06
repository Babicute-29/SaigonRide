using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaigonRide.Models
{
    public class SupportReport
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int? VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? AdminReply { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Resolved

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}