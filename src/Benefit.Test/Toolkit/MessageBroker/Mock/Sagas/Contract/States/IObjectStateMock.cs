using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
public record IObjectStateMock
{
    public Guid CorrelationId { get; set; }
    public string Name { get; set; }
}