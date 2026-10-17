namespace SmartParkingAPI.DTOs
{
    public class ParkingSpotDto
    {
        public int Id { get; set; }

        public string SpotNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public int ParkingLotId { get; set; }
    }
}