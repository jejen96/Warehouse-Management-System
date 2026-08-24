using FluentValidation;
using WMS.Application.DTOs.MasterData;

namespace WMS.Application.Validators.MasterData;

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseValidator()
    {
        RuleFor(x => x.WarehouseCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.WarehouseName).NotEmpty().MaximumLength(200);
    }
}

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.LocationCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.WarehouseId).NotEmpty();
    }
}
