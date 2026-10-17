using SmartParkingAPI.Models;

namespace SmartParkingAPI.Repositories
{
    public interface IParkingLotRepository
    {
        Task<IEnumerable<ParkingLot>> GetAllAsync(
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize
        );

        Task<ParkingLot?> GetByIdAsync(int id);
        Task<ParkingLot> CreateAsync(ParkingLot parkingLot);
        Task<bool> UpdateAsync(ParkingLot parkingLot);
        Task<bool> DeleteAsync(int id);
    }
}