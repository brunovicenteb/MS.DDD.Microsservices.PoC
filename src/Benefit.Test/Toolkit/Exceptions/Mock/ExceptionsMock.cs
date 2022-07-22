using System.Collections;
using System.Collections.Generic;
using Toolkit.Exceptions;

namespace Benefit.Test.Toolkit.Exceptions.Mock
{
    public class ExceptionsMock : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {

            yield return new object[] { new NotFoundException("NotFoundException") };
            yield return new object[] { new ForbidException("ForbidException") };
            yield return new object[] { new UnauthorizedException("UnauthorizedException") };
            yield return new object[] { new BadRequestException("BadRequestException") };
            yield return new object[] { new DomainRuleException("DomainRuleException") };
            yield return new object[] { new DuplicateRegistrationException("DuplicateRegistrationException") };
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
