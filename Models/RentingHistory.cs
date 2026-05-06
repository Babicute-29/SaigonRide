using System.ComponentModel.DataAnnotations.Schema;

namespace SaigonRide.Models
{
    public class RentingHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // --- LIÊN KẾT VỚI BẢNG VEHICLE ---
        public int VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        // --- LIÊN KẾT VỚI TRẠM LẤY XE (PICKUP) ---
        public int PickupStationId { get; set; }
        [ForeignKey("PickupStationId")]
        public virtual Station? PickupStation { get; set; }

        // --- THÊM CỘT NÀY: LIÊN KẾT VỚI TRẠM TRẢ XE (RETURN) ---
        // Để int? (nullable) vì lúc mới thuê chưa biết sẽ trả ở trạm nào
        public int? ReturnStationId { get; set; }
        [ForeignKey("ReturnStationId")]
        public virtual Station? ReturnStation { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // Sửa lỗi Non-nullable bằng cách gán chuỗi rỗng mặc định
        public string Status { get; set; } = "In Progress";

        public decimal? TotalPrice { get; set; }
    }
}