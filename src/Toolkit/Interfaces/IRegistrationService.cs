using Toolkit.TransactionalOutBox;
using Toolkit.MessageBroker.TransactionOutbox;

namespace Toolkit.Interfaces;

public interface IRegistrationService
{
    Task<Registration> SubmitRegistration(string eventId, string memberId, decimal payment);
}