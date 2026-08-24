using FluentValidation;
using WMS.Application.DTOs.MasterData;

namespace WMS.Application.Validators.MasterData;

public class CreateItemValidator : AbstractValidator<CreateItemDto>
{
    public CreateItemValidator()
    {
        RuleFor(x => x.ItemCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UOM).NotEmpty().MaximumLength(20);
        RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxStock).GreaterThan(0).GreaterThanOrEqualTo(x => x.MinStock);
    }
}

public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UOM).NotEmpty().MaximumLength(20);
        RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxStock).GreaterThan(0).GreaterThanOrEqualTo(x => x.MinStock);
    }
}
