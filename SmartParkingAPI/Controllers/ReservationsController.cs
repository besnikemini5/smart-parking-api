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
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<ReservationsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
        {
            _logger.LogInformation("Getting all reservations.");

            var reservations = await _context.Reservations.ToListAsync();

            var reservationDtos = _mapper.Map<IEnumerable<ReservationDto>>(reservations);

            return Ok(reservationDtos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            _logger.LogInformation("Getting reservation with ID: {ReservationId}", id);

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation with ID {ReservationId} was not found.", id);
                return NotFound();
            }

            var reservationDto = _mapper.Map<ReservationDto>(reservation);

            return Ok(reservationDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationDto createDto)
        {
            if (createDto.EndTime <= createDto.StartTime)
            {
                _logger.LogWarning("Invalid reservation time. End time is before or equal to start time.");
                return BadRequest("End time must be after start time.");
            }

            var parkingSpot = await _context.ParkingSpots.FindAsync(createDto.ParkingSpotId);

            if (parkingSpot == null)
            {
                _logger.LogWarning("Cannot create reservation. Parking spot ID {ParkingSpotId} does not exist.", createDto.ParkingSpotId);
                return BadRequest("Parking spot does not exist.");
            }

            if (!parkingSpot.IsAvailable)
            {
                _logger.LogWarning("Cannot create reservation. Parking spot ID {ParkingSpotId} is not available.", createDto.ParkingSpotId);
                return BadRequest("Parking spot is not available.");
            }

            var reservation = _mapper.Map<Reservation>(createDto);

            parkingSpot.IsAvailable = false;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created reservation with ID: {ReservationId}", reservation.Id);

            var reservationDto = _mapper.Map<ReservationDto>(reservation);

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservationDto);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, CreateReservationDto updateDto)
        {
            if (updateDto.EndTime <= updateDto.StartTime)
            {
                _logger.LogWarning("Invalid reservation update. End time is before or equal to start time.");
                return BadRequest("End time must be after start time.");
            }

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                _logger.LogWarning("Cannot update. Reservation with ID {ReservationId} was not found.", id);
                return NotFound();
            }

            var parkingSpot = await _context.ParkingSpots.FindAsync(updateDto.ParkingSpotId);

            if (parkingSpot == null)
            {
                _logger.LogWarning("Cannot update reservation. Parking spot ID {ParkingSpotId} does not exist.", updateDto.ParkingSpotId);
                return BadRequest("Parking spot does not exist.");
            }

            _mapper.Map(updateDto, reservation);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated reservation with ID: {ReservationId}", id);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            _logger.LogInformation("Deleting reservation with ID: {ReservationId}", id);

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                _logger.LogWarning("Cannot delete. Reservation with ID {ReservationId} was not found.", id);
                return NotFound();
            }

            var parkingSpot = await _context.ParkingSpots.FindAsync(reservation.ParkingSpotId);

            if (parkingSpot != null)
            {
                parkingSpot.IsAvailable = true;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted reservation with ID: {ReservationId}", id);

            return NoContent();
        }
    }
}