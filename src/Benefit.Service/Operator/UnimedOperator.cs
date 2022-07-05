using Benefit.Domain.Benefit;

namespace Benefit.Service.Operator;

public sealed class UnimedOperator : BaseOperatorService
{
    protected override OperatorType Type => OperatorType.Unimed;
    protected override bool IsRequiredCPF(Beneficiary beneficiary) => !beneficiary.IsUnderAge;
}