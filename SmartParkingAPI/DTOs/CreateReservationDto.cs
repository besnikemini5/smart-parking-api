namespace SmartParkingAPI.DTOs
{
    public class CreateReservationDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public string VehiclePlateNumber { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int ParkingSpotId { get; set; }
    }
}