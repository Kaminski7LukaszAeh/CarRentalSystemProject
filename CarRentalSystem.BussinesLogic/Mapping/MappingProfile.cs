using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Filters;

namespace CarRentalSystem.BusinessLogic.Mapping
{
    public static class DtoToEntityMapper
    {
        public static ApplicationUser Map(RegisterDto dto)
        {
            return new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };
        }

        public static Vehicle Map(CreateVehicleDto dto)
        {
            return new Vehicle
            {
                DailyRate = dto.DailyRate,
                IsAvailable = true,
                VehicleModelId = dto.VehicleModelId,
            };
        }

        public static VehicleType Map(CreateVehicleTypeDto dto)
        {
            return new VehicleType
            {
                TypeName = dto.TypeName
            };
        }

        public static VehicleType Map(VehicleTypeDto dto)
        {
            return new VehicleType
            {
                Id = dto.Id,
                TypeName = dto.TypeName
            };
        }

        public static VehicleBrand Map(CreateVehicleBrandDto dto)
        {
            return new VehicleBrand
            {
                Name = dto.Name
            };
        }

        public static VehicleModel Map(CreateVehicleModelDto dto)
        {
            return new VehicleModel
            {
                Name = dto.Name,
                BrandId = dto.BrandId,
                VehicleTypeId = dto.VehicleTypeId
            };
        }
        public static Reservation Map(CreateReservationDto dto)
        {
            return new Reservation
            {
                VehicleId = dto.VehicleId,
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow.AddHours(2),
                Status = ReservationStatus.Active,
                TotalCost = dto.TotalCost,
                IsReturned = dto.IsReturned,
                Payment = new Payment
                {
                    Amount = dto.TotalCost,
                    Status = PaymentStatus.Pending,
                    PaymentConfirmationNumber = "Not Paid Yet"
                }
            };
        }
        public static VehicleFilterDal Map(VehicleFilterDto dto)
        {
            return new VehicleFilterDal
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice,
                SelectedVehicleTypeIds = dto.SelectedVehicleTypeIds
            };
        }
        public static Payment Map(PaymentDto dto)
        {
            return new Payment
            {
                Id = dto.Id,
                ReservationId = dto.ReservationId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Status = dto.Status,
                PaymentConfirmationNumber = dto.PaymentConfirmationNumber
            };
        }
    }

    public static class EntityToDtoMapper
    {
        public static VehicleDto Map(Vehicle entity)
        {
            return new VehicleDto
            {
                Id = entity.Id,
                DailyRate = entity.DailyRate,
                IsAvailable = entity.IsAvailable,
                VehicleModelId = entity.VehicleModelId,
                VehicleModelName = entity.VehicleModel?.Name ?? "N/A",
                VehicleTypeId = entity.VehicleModel?.VehicleTypeId ?? 0,
                VehicleTypeName = entity.VehicleModel?.VehicleType?.TypeName ?? "N/A",
                VehicleBrandId = entity.VehicleModel?.BrandId ?? 0,
                VehicleBrandName = entity.VehicleModel?.Brand?.Name ?? "N/A"
            };
        }

        public static List<VehicleDto> MapList(IEnumerable<Vehicle> entities) =>
            entities.Select(Map).ToList();

        public static VehicleTypeDto Map(VehicleType entity)
        {
            return new VehicleTypeDto
            {
                Id = entity.Id,
                TypeName = entity.TypeName
            };
        }

        public static List<VehicleTypeDto> MapList(IEnumerable<VehicleType> entities)
        {
            return entities.Select(Map).ToList();
        }

        public static VehicleBrandDto Map(VehicleBrand entity)
        {
            return new VehicleBrandDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static List<VehicleBrandDto> MapList(IEnumerable<VehicleBrand> entities)
        {
            return entities.Select(Map).ToList();
        }

        public static VehicleModelDto Map(VehicleModel entity)
        {
            return new VehicleModelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                BrandId = entity.BrandId,
                BrandName = entity.Brand?.Name ?? "Unknown",
                VehicleTypeId = entity.VehicleTypeId,
                VehicleTypeName = entity.VehicleType?.TypeName ?? "N/A"
            };
        }

        public static List<VehicleModelDto> MapList(IEnumerable<VehicleModel> entities)
        {
            return entities.Select(Map).ToList();
        }
        public static ReservationDto Map(Reservation entity)
        {
            return new ReservationDto
            {
                Id = entity.Id,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CreatedAt = entity.CreatedAt,
                TotalCost = entity.TotalCost,
                VehicleId = entity.VehicleId,
                DailyRate = entity.Vehicle?.DailyRate ?? 0,
                BrandName = entity.Vehicle?.VehicleModel?.Brand?.Name ?? "Unknown",
                ModelName = entity.Vehicle?.VehicleModel?.Name ?? "Unknown",
                VehicleTypeName = entity.Vehicle?.VehicleModel?.VehicleType?.TypeName ?? "Unknown",
                Status = entity.Status,
                PaymentStatus = entity.Payment?.Status ?? PaymentStatus.Pending,
                IsReturned = entity.IsReturned
            };
        }

        public static PaymentDto Map(Payment entity)
        {
            return new PaymentDto
            {
                Id = entity.Id,
                ReservationId = entity.ReservationId,
                Amount = entity.Amount,
                PaymentDate = entity.PaymentDate,
                Status = entity.Status,
                PaymentConfirmationNumber = entity.PaymentConfirmationNumber
            };
        }

        public static List<ReservationDto> MapList(IEnumerable<Reservation> entities)
        {
            return entities.Select(Map).ToList();
        }
    }
}
