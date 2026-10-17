namespace SmartParkingAPI.DTOs
{
    public class CreateParkingSpotDto
    {
        public string SpotNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public int ParkingLotId { get; set; }
    }
}