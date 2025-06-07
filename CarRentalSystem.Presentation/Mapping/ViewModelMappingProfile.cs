using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.Presentation.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarRentalSystem.Presentation.Mapping
{
    public static class ViewModelToDtoMapper
    {
        public static RegisterDto Map(RegisterViewModel vm)
        {
            return new RegisterDto
            {
                Email = vm.Email,
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword
            };
        }

        public static LoginDto Map(LoginViewModel vm)
        {
            return new LoginDto
            {
                Email = vm.Email,
                Password = vm.Password,
                RememberMe = vm.RememberMe
            };
        }

        public static CreateVehicleTypeDto Map(CreateVehicleTypeViewModel vm)
        {
            return new CreateVehicleTypeDto
            {
                TypeName = vm.TypeName
            };
        }

        public static VehicleTypeDto Map(VehicleTypeViewModel vm)
        {
            return new VehicleTypeDto
            {
                Id = vm.Id,
                TypeName = vm.TypeName
            };
        }

        public static CreateVehicleBrandDto Map(CreateVehicleBrandViewModel vm)
        {
            return new CreateVehicleBrandDto
            {
                Name = vm.Name
            };
        }

        public static VehicleBrandDto Map(VehicleBrandViewModel vm)
        {
            return new VehicleBrandDto
            {
                Id = vm.Id,
                Name = vm.Name
            };
        }

        public static CreateVehicleModelDto Map(CreateVehicleModelViewModel vm)
        {
            return new CreateVehicleModelDto
            {
                Name = vm.Name,
                BrandId = vm.BrandId,
                VehicleTypeId = vm.VehicleTypeId
            };
        }

        public static VehicleModelDto Map(VehicleModelViewModel vm)
        {
            return new VehicleModelDto
            {
                Id = vm.Id,
                Name = vm.Name,
                BrandId = vm.BrandId,
                BrandName = vm.BrandName,
                VehicleTypeId = vm.VehicleTypeId,
                VehicleTypeName = vm.VehicleTypeName
            };
        }
        public static CreateVehicleDto Map(CreateVehicleViewModel model)
        {
            return new CreateVehicleDto
            {
                DailyRate = model.DailyRate,
                VehicleModelId = model.VehicleModelId,
                VehicleTypeId = model.VehicleTypeId,
            };
        }
        public static CreateReservationDto Map(CreateReservationViewModel vm, string userId)
        {
            return new CreateReservationDto
            {
                VehicleId = vm.VehicleId,
                UserId = userId,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                TotalCost = vm.TotalCost,
                IsReturned = vm.IsReturned
            };
        }
        public static PaymentDto Map(PaymentViewModel vm)
        {
            return new PaymentDto
            {
                ReservationId = vm.ReservationId,
                Amount = vm.Amount,
                PaymentDate = vm.PaymentDate ?? DateTime.UtcNow,
                Status = vm.Status ?? PaymentStatus.Pending, 
                PaymentConfirmationNumber = vm.PaymentConfirmationNumber
            };
        }
    }
}

public static class DtoToViewModelMapper
{
    public static VehicleViewModel Map(VehicleDto dto)
    {
        return new VehicleViewModel
        {
            Id = dto.Id,
            DailyRate = dto.DailyRate,
            IsAvailable = dto.IsAvailable,
            ModelName = dto.VehicleModelName,
            TypeName = dto.VehicleTypeName,
            BrandName = dto.VehicleBrandName
        };
    }

    public static VehicleTypeViewModel Map(VehicleTypeDto dto)
    {
        return new VehicleTypeViewModel
        {
            Id = dto.Id,
            TypeName = dto.TypeName
        };
    }

    public static List<VehicleTypeViewModel> MapList(IEnumerable<VehicleTypeDto> dtos)
    {
        return dtos.Select(Map).ToList();
    }

    public static VehicleBrandViewModel Map(VehicleBrandDto dto)
    {
        return new VehicleBrandViewModel
        {
            Id = dto.Id,
            Name = dto.Name
        };
    }
    public static PaymentViewModel Map(PaymentDto dto)
    {
        return new PaymentViewModel
        {
            ReservationId = dto.ReservationId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            Status = dto.Status,
            PaymentConfirmationNumber = dto.PaymentConfirmationNumber
        };
    }

    public static List<VehicleBrandViewModel> MapList(IEnumerable<VehicleBrandDto> dtos)
    {
        return dtos.Select(Map).ToList();
    }

    public static VehicleModelViewModel Map(VehicleModelDto dto)
    {
        return new VehicleModelViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            BrandId = dto.BrandId,
            BrandName = dto.BrandName,
            VehicleTypeId = dto.VehicleTypeId,
            VehicleTypeName = dto.VehicleTypeName
        };
    }

    public static List<VehicleModelViewModel> MapList(IEnumerable<VehicleModelDto> dtos)
    {
        return dtos.Select(Map).ToList();
    }

    public static SelectListItem MapToSelectListItem(VehicleBrandDto dto)
    {
        return new SelectListItem
        {
            Value = dto.Id.ToString(),
            Text = dto.Name
        };
    }
    public static ReservationViewModel Map(ReservationDto dto)
    {
        return new ReservationViewModel
        {
            Id = dto.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalCost = dto.TotalCost,
            VehicleId = dto.VehicleId,
            DailyRate = dto.DailyRate,
            BrandName = dto.BrandName,
            ModelName = dto.ModelName,
            VehicleTypeName = dto.VehicleTypeName,
            CreatedAt = dto.CreatedAt,
            Status = dto.Status,
            UserId = dto.UserId,
            PaymentStatus = dto.PaymentStatus,
            IsReturned = dto.IsReturned
        };
    }


    public static List<ReservationViewModel> MapList(IEnumerable<ReservationDto> dtos)
    {
        return dtos.Select(Map).ToList();
    }


    public static List<SelectListItem> MapBrandsToSelectList(IEnumerable<VehicleBrandDto> brandDtos)
    {
        return brandDtos.Select(MapToSelectListItem).ToList();
    }

    public static List<VehicleViewModel> MapList(IEnumerable<VehicleDto> dtos) =>
        dtos.Select(Map).ToList();

    public static List<SelectListItem> MapModelsToSelectList(IEnumerable<VehicleModelDto> models) =>
        models.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }).ToList();

    public static List<SelectListItem> MapTypesToSelectList(IEnumerable<VehicleTypeDto> types) =>
        types.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.TypeName }).ToList();
}

public static class FilterMapper
{
    public static VehicleFilterDto Map(VehicleFilter filter)
    {
        if (filter == null) return null;

        return new VehicleFilterDto
        {
            StartDate = filter.StartDate,
            EndDate = filter.EndDate,
            MinPrice = filter.MinPrice,
            MaxPrice = filter.MaxPrice,
            SelectedVehicleTypeIds = filter.SelectedVehicleTypeIds

        };
    }
}

