using Toolkit;
using FluentValidation;

namespace Benefit.Domain.Benefit.Rules;
public class BeneficiaryValidator : AbstractValidator<Beneficiary>
{
    public BeneficiaryValidator()
    {
        RuleFor(o => o.Name)
            .NotNull()
            .NotEmpty().WithMessage("The field \"Name\" cannot be empty.");
        RuleFor(o => o.CPF)
            .Must(o => o.IsValidCPF()).WithMessage("The field \"CPF\" is not valid.");
        RuleFor(o => o.BirthDate)
            .Must(o => o.Value > DateTime.Now).WithMessage("The field \"BirthDate\" cannot be in the future.");
    }
}