using Benefit.Domain.Benefit;

namespace Benefit.Service.Operator;
public sealed class AmilOperator : BaseOperatorService
{
    protected override OperatorType Type => OperatorType.Amil;
}