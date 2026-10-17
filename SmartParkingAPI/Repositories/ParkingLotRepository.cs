using Microsoft.EntityFrameworkCore;
using SmartParkingAPI.Data;
using SmartParkingAPI.Models;

namespace SmartParkingAPI.Repositories
{
    public class ParkingLotRepository : IParkingLotRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingLotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParkingLot>> GetAllAsync(
            string? search,
            string? sortBy,
            bool descending,
            int pageNumber,
            int pageSize
        )
        {
            var query = _context.ParkingLots.AsQueryable();

            // Filtering by name or location
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Location.Contains(search));
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "location" => descending ? query.OrderByDescending(p => p.Location) : query.OrderBy(p => p.Location),
                "price" => descending ? query.OrderByDescending(p => p.PricePerHour) : query.OrderBy(p => p.PricePerHour),
                "totalspots" => descending ? query.OrderByDescending(p => p.TotalSpots) : query.OrderBy(p => p.TotalSpots),
                _ => query.OrderBy(p => p.Id)
            };

            // Pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ParkingLot?> GetByIdAsync(int id)
        {
            return await _context.ParkingLots.FindAsync(id);
        }

        public async Task<ParkingLot> CreateAsync(ParkingLot parkingLot)
        {
            _context.ParkingLots.Add(parkingLot);
            await _context.SaveChangesAsync();
            return parkingLot;
        }

        public async Task<bool> UpdateAsync(ParkingLot parkingLot)
        {
            _context.ParkingLots.Update(parkingLot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);

            if (parkingLot == null)
            {
                return false;
            }

            _context.ParkingLots.Remove(parkingLot);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}