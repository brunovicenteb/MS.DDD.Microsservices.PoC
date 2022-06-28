using Benefit.Domain.AggregatesModel.BeneficiaryAggregate;

namespace Benefit.Domain.Operator
{
    public sealed class UnimedOperator : BaseOperator
    {
        protected override OperatorType Type => OperatorType.Unimed;
        protected override bool IsRequiredCPF(Beneficiary beneficiary) => beneficiary.IsTitular || !beneficiary.IsUnderAge;
    }
}