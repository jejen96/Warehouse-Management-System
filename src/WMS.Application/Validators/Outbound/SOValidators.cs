using FluentValidation;
using WMS.Application.DTOs.Outbound;

namespace WMS.Application.Validators.Outbound;

public class CreateSOValidator : AbstractValidator<CreateSODto>
{
    public CreateSOValidator()
    {
        RuleFor(x => x.SODate).NotEmpty();
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Details).NotEmpty().WithMessage("SO must have at least one line item.");
        RuleForEach(x => x.Details).SetValidator(new CreateSODetailValidator());
    }
}

public class CreateSODetailValidator : AbstractValidator<CreateSODetailDto>
{
    public CreateSODetailValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.OrderedQty).GreaterThan(0);
        RuleFor(x => x.UOM).NotEmpty();
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
