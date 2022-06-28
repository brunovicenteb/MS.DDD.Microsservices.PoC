using FluentValidation;

namespace Benefit.Domain.AggregatesModel.BeneficiaryAggregate.Rules
{
    public class BeneficiaryValidator : AbstractValidator<Beneficiary>
    {
        public BeneficiaryValidator()
        {
            RuleFor(beneficiary => beneficiary.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("Invalid beneficiary name!");

            RuleFor(beneficiary => beneficiary.CPF)
                .NotEmpty()
                .NotNull()
                .WithMessage("Invalid beneficiary CPF!");

            RuleFor(beneficiary => beneficiary.BirthDate)
                .NotEmpty()
                .NotNull()
                .WithMessage("Invalid beneficiary Birth Date!");
        }
    }
}
