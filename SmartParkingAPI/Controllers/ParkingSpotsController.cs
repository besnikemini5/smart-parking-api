using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingAPI.Data;
using SmartParkingAPI.DTOs;
using SmartParkingAPI.Models;

namespace SmartParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpotsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ParkingSpotsController> _logger;

        public ParkingSpotsController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ParkingSpotsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingSpotDto>>> GetParkingSpots()
        {
            _logger.LogInformation("Getting all parking spots.");

            var spots = await _context.ParkingSpots.ToListAsync();

            var spotDtos = _mapper.Map<IEnumerable<ParkingSpotDto>>(spots);

            return Ok(spotDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingSpotDto>> GetParkingSpot(int id)
        {
            _logger.LogInformation("Getting parking spot with ID: {ParkingSpotId}", id);

            var spot = await _context.ParkingSpots.FindAsync(id);

            if (spot == null)
            {
                _logger.LogWarning("Parking spot with ID {ParkingSpotId} was not found.", id);
                return NotFound();
            }

            var spotDto = _mapper.Map<ParkingSpotDto>(spot);

            return Ok(spotDto);
        }

        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<ParkingSpotDto>>> GetAvailableSpots()
        {
            _logger.LogInformation("Getting available parking spots.");

            var spots = await _context.ParkingSpots
                .Where(s => s.IsAvailable)
                .ToListAsync();

            var spotDtos = _mapper.Map<IEnumerable<ParkingSpotDto>>(spots);

            return Ok(spotDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ParkingSpotDto>> CreateParkingSpot(CreateParkingSpotDto createDto)
        {
            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(p => p.Id == createDto.ParkingLotId);

            if (!parkingLotExists)
            {
                _logger.LogWarning("Cannot create parking spot. Parking lot ID {ParkingLotId} does not exist.", createDto.ParkingLotId);
                return BadRequest("Parking lot does not exist.");
            }

            var spot = _mapper.Map<ParkingSpot>(createDto);

            _context.ParkingSpots.Add(spot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created parking spot with ID: {ParkingSpotId}", spot.Id);

            var spotDto = _mapper.Map<ParkingSpotDto>(spot);

            return CreatedAtAction(nameof(GetParkingSpot), new { id = spot.Id }, spotDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingSpot(int id, CreateParkingSpotDto updateDto)
        {
            _logger.LogInformation("Updating parking spot with ID: {ParkingSpotId}", id);

            var spot = await _context.ParkingSpots.FindAsync(id);

            if (spot == null)
            {
                _logger.LogWarning("Cannot update. Parking spot with ID {ParkingSpotId} was not found.", id);
                return NotFound();
            }

            var parkingLotExists = await _context.ParkingLots
                .AnyAsync(p => p.Id == updateDto.ParkingLotId);

            if (!parkingLotExists)
            {
                _logger.LogWarning("Cannot update parking spot. Parking lot ID {ParkingLotId} does not exist.", updateDto.ParkingLotId);
                return BadRequest("Parking lot does not exist.");
            }

            _mapper.Map(updateDto, spot);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated parking spot with ID: {ParkingSpotId}", id);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpot(int id)
        {
            _logger.LogInformation("Deleting parking spot with ID: {ParkingSpotId}", id);

            var spot = await _context.ParkingSpots.FindAsync(id);

            if (spot == null)
            {
                _logger.LogWarning("Cannot delete. Parking spot with ID {ParkingSpotId} was not found.", id);
                return NotFound();
            }

            _context.ParkingSpots.Remove(spot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted parking spot with ID: {ParkingSpotId}", id);

            return NoContent();
        }
    }
}