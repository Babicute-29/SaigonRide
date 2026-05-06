namespace SaigonRide.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Status { get; set; } = "Active";
    }
}