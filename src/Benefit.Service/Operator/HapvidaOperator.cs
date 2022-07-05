using Benefit.Domain.Benefit;

namespace Benefit.Service.Operator;
public sealed class HapvidaOperator : BaseOperatorService
{
    protected override OperatorType Type => OperatorType.Hapvida;
}