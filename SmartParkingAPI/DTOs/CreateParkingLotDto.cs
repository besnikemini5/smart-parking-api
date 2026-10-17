namespace SmartParkingAPI.DTOs
{
    public class CreateParkingLotDto
    {
        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int TotalSpots { get; set; }

        public decimal PricePerHour { get; set; }
    }
}