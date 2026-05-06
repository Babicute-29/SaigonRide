namespace SaigonRide.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string Type { get; set; } = "Standard"; // Standard hoặc E-Scooter
        public string Status { get; set; } = "Available";
        public int StationId { get; set; }
    }
}