using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartParkingAPI.DTOs;
using SmartParkingAPI.Models;
using SmartParkingAPI.Repositories;

namespace SmartParkingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingLotsController : ControllerBase
    {
        private readonly IParkingLotRepository _parkingLotRepository;
        private readonly ILogger<ParkingLotsController> _logger;
        private readonly IMapper _mapper;

        public ParkingLotsController(
            IParkingLotRepository parkingLotRepository,
            ILogger<ParkingLotsController> logger,
            IMapper mapper)
        {
            _parkingLotRepository = parkingLotRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingLotDto>>> GetParkingLots(
            string? search,
            string? sortBy = "id",
            bool descending = false,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            if (pageNumber < 1)
            {
                _logger.LogWarning("Invalid page number: {PageNumber}", pageNumber);
                return BadRequest("Page number must be greater than 0.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                _logger.LogWarning("Invalid page size: {PageSize}", pageSize);
                return BadRequest("Page size must be between 1 and 100.");
            }

            _logger.LogInformation(
                "Getting parking lots with search: {Search}, sortBy: {SortBy}, descending: {Descending}, pageNumber: {PageNumber}, pageSize: {PageSize}",
                search,
                sortBy,
                descending,
                pageNumber,
                pageSize
            );

            var parkingLots = await _parkingLotRepository.GetAllAsync(
                search,
                sortBy,
                descending,
                pageNumber,
                pageSize
            );

            var parkingLotDtos = _mapper.Map<IEnumerable<ParkingLotDto>>(parkingLots);

            return Ok(parkingLotDtos);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingLotDto>> GetParkingLot(int id)
        {
            _logger.LogInformation("Getting parking lot with ID: {ParkingLotId}", id);

            var parkingLot = await _parkingLotRepository.GetByIdAsync(id);

            if (parkingLot == null)
            {
                _logger.LogWarning("Parking lot with ID {ParkingLotId} was not found.", id);
                return NotFound();
            }

            var parkingLotDto = _mapper.Map<ParkingLotDto>(parkingLot);

            return Ok(parkingLotDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ParkingLotDto>> CreateParkingLot(CreateParkingLotDto createDto)
        {
            var parkingLot = _mapper.Map<ParkingLot>(createDto);

            parkingLot.IsActive = true;

            var createdParkingLot = await _parkingLotRepository.CreateAsync(parkingLot);

            _logger.LogInformation("Created parking lot with ID: {ParkingLotId}", createdParkingLot.Id);

            var parkingLotDto = _mapper.Map<ParkingLotDto>(createdParkingLot);

            return CreatedAtAction(
                nameof(GetParkingLot),
                new { id = createdParkingLot.Id },
                parkingLotDto
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingLot(int id, CreateParkingLotDto updateDto)
        {
            _logger.LogInformation("Updating parking lot with ID: {ParkingLotId}", id);

            var parkingLot = await _parkingLotRepository.GetByIdAsync(id);

            if (parkingLot == null)
            {
                _logger.LogWarning("Cannot update. Parking lot with ID {ParkingLotId} was not found.", id);
                return NotFound();
            }

            _mapper.Map(updateDto, parkingLot);

            await _parkingLotRepository.UpdateAsync(parkingLot);

            _logger.LogInformation("Updated parking lot with ID: {ParkingLotId}", id);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(int id)
        {
            _logger.LogInformation("Deleting parking lot with ID: {ParkingLotId}", id);

            var deleted = await _parkingLotRepository.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Cannot delete. Parking lot with ID {ParkingLotId} was not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted parking lot with ID: {ParkingLotId}", id);

            return NoContent();
        }
    }
}