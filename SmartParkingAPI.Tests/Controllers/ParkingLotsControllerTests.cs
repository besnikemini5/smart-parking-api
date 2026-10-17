using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartParkingAPI.Controllers;
using SmartParkingAPI.DTOs;
using SmartParkingAPI.Mappings;
using SmartParkingAPI.Models;
using SmartParkingAPI.Repositories;

namespace SmartParkingAPI.Tests.Controllers
{
    public class ParkingLotsControllerTests
    {
        private readonly Mock<IParkingLotRepository> _repositoryMock;
        private readonly Mock<ILogger<ParkingLotsController>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly ParkingLotsController _controller;

        public ParkingLotsControllerTests()
        {
            _repositoryMock = new Mock<IParkingLotRepository>();
            _loggerMock = new Mock<ILogger<ParkingLotsController>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _controller = new ParkingLotsController(
                _repositoryMock.Object,
                _loggerMock.Object,
                _mapper
            );
        }

        [Fact]
        public async Task GetParkingLots_ReturnsOk_WithParkingLots()
        {
            var parkingLots = new List<ParkingLot>
            {
                new ParkingLot
                {
                    Id = 1,
                    Name = "Central Parking",
                    Location = "Skopje Center",
                    TotalSpots = 100,
                    PricePerHour = 2.5m,
                    IsActive = true
                },
                new ParkingLot
                {
                    Id = 2,
                    Name = "City Mall Parking",
                    Location = "Karposh",
                    TotalSpots = 150,
                    PricePerHour = 3.0m,
                    IsActive = true
                }
            };

            _repositoryMock
                .Setup(repo => repo.GetAllAsync(null, "id", false, 1, 10))
                .ReturnsAsync(parkingLots);

            var result = await _controller.GetParkingLots(null);

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedLots = okResult.Value.Should().BeAssignableTo<IEnumerable<ParkingLotDto>>().Subject;

            returnedLots.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetParkingLots_InvalidPageNumber_ReturnsBadRequest()
        {
            var result = await _controller.GetParkingLots(null, pageNumber: 0);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetParkingLot_NonExistingId_ReturnsNotFound()
        {
            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((ParkingLot?)null);

            var result = await _controller.GetParkingLot(99);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateParkingLot_ValidData_ReturnsCreatedAtAction()
        {
            var createDto = new CreateParkingLotDto
            {
                Name = "New Parking",
                Location = "Tetovo",
                TotalSpots = 80,
                PricePerHour = 2.0m
            };

            var createdParkingLot = new ParkingLot
            {
                Id = 3,
                Name = "New Parking",
                Location = "Tetovo",
                TotalSpots = 80,
                PricePerHour = 2.0m,
                IsActive = true
            };

            _repositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<ParkingLot>()))
                .ReturnsAsync(createdParkingLot);

            var result = await _controller.CreateParkingLot(createDto);

            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;

            createdResult.ActionName.Should().Be(nameof(ParkingLotsController.GetParkingLot));
            createdResult.RouteValues!["id"].Should().Be(3);
        }

        [Fact]
        public async Task DeleteParkingLot_NonExistingId_ReturnsNotFound()
        {
            _repositoryMock
                .Setup(repo => repo.DeleteAsync(99))
                .ReturnsAsync(false);

            var result = await _controller.DeleteParkingLot(99);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}