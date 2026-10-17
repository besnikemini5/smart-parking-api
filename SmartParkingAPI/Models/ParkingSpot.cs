namespace SmartParkingAPI.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }

        public string SpotNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public int ParkingLotId { get; set; }

        public ParkingLot? ParkingLot { get; set; }
    }
}