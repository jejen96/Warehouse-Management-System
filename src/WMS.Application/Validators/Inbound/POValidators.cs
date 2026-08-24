using FluentValidation;
using WMS.Application.DTOs.Inbound;

namespace WMS.Application.Validators.Inbound;

public class CreatePOValidator : AbstractValidator<CreatePODto>
{
    public CreatePOValidator()
    {
        RuleFor(x => x.PODate).NotEmpty();
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.Details).NotEmpty().WithMessage("PO must have at least one line item.");
        RuleForEach(x => x.Details).SetValidator(new CreatePODetailValidator());
    }
}

public class CreatePODetailValidator : AbstractValidator<CreatePODetailDto>
{
    public CreatePODetailValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.OrderedQty).GreaterThan(0);
        RuleFor(x => x.UOM).NotEmpty();
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}

public class CreateGRNValidator : AbstractValidator<CreateGRNDto>
{
    public CreateGRNValidator()
    {
        RuleFor(x => x.GRNDate).NotEmpty();
        RuleFor(x => x.POId).NotEmpty();
        RuleFor(x => x.ReceivedBy).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Details).NotEmpty().WithMessage("GRN must have at least one line item.");
        RuleForEach(x => x.Details).SetValidator(new CreateGRNDetailValidator());
    }
}

public class CreateGRNDetailValidator : AbstractValidator<CreateGRNDetailDto>
{
    public CreateGRNDetailValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.ReceivedQty).GreaterThan(0);
    }
}
