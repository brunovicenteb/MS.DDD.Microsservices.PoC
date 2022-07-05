using Toolkit.Exceptions;

namespace Benefit.Domain.Operator;

public static class OperatorFactory
{
    public static BaseOperator CreateOperator(OperatorType type)
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