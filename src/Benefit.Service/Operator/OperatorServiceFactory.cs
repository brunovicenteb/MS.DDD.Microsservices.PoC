using Toolkit.Exceptions;
using Benefit.Domain.Benefit;

namespace Benefit.Service.Operator;

public static class OperatorServiceFactory
{
    public static BaseOperatorService CreateOperator(OperatorType type)
    {
        switch (type)
        {
            case OperatorType.Amil:
                return new AmilOperator();
            case OperatorType.Unimed:
                return new UnimedOperator();
            case OperatorType.Hapvida:
                return new HapvidaOperator();
        }
        throw new DomainRuleException($"The operator {type} is not supported.");
    }
}