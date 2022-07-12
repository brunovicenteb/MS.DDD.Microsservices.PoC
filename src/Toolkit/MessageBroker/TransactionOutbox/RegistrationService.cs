using Npgsql;
using MassTransit;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Microsoft.EntityFrameworkCore;
using Toolkit.TransactionalOutBox;

namespace Toolkit.MessageBroker.TransactionOutbox;

public class RegistrationService<T> : IRegistrationService where T : TransactionalOutBoxDbContext
{
    private readonly T _Context;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegistrationService(T context, IPublishEndpoint publishEndpoint)
    {
        _Context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Registration> SubmitRegistration(string eventId, string memberId, decimal payment)
    {
        var registration = new Registration
        {
            RegistrationId = NewId.NextGuid(),
            RegistrationDate = DateTime.UtcNow,
            MemberId = memberId,
            EventId = eventId,
        };

        await _Context.Set<Registration>().AddAsync(registration);

        await _publishEndpoint.Publish(new RegistrationSubmitted
        {
            RegistrationId = registration.RegistrationId,
            RegistrationDate = registration.RegistrationDate,
            MemberId = registration.MemberId,
            EventId = registration.EventId,
            Payment = payment
        });

        try
        {
            await _Context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new DuplicateRegistrationException("Duplicate registration", exception);
        }

        return registration;
    }
}