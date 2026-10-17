using AutoMapper;
using SmartParkingAPI.DTOs;
using SmartParkingAPI.Models;

namespace SmartParkingAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ParkingLot, ParkingLotDto>();
            CreateMap<CreateParkingLotDto, ParkingLot>();

            CreateMap<ParkingSpot, ParkingSpotDto>();
            CreateMap<CreateParkingSpotDto, ParkingSpot>();

            CreateMap<Reservation, ReservationDto>();
            CreateMap<CreateReservationDto, Reservation>();
        }
    }
}