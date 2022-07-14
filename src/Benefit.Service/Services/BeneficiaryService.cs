using MassTransit;
using Toolkit.Mapper;
using Toolkit.Interfaces;
using Benefit.Domain.Benefit;
using Benefit.Domain.Operator;
using Benefit.Domain.Interfaces;
using Benefit.Service.Interfaces;
using Benefit.Domain.AggregatesModel.Benefit;
using Benefit.Service.Infra;
using Microsoft.EntityFrameworkCore;
using Toolkit.Exceptions;

namespace Benefit.Service.Services;

public class BeneficiaryService : IBeneficiaryService
{
    public BeneficiaryService(IPublishEndpoint publiser, IBenefitRepository benefitRepository)
    {
        _Publisher = publiser;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Map<Beneficiary, BeneficiarySubmitted>();
    }

    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly IGenericMapper _Mapper;
    private static Guid? _CorrerID;

    public async Task<Beneficiary> SubmitBeneficiary(OperatorType operatorType, string name, string cpf, DateTime? birthDate)
    {
        if (!_CorrerID.HasValue)
            _CorrerID = NewId.NextGuid();
        var op = Operator.CreateOperator(operatorType);
        var entity = op.CreateBeneficiary(name, cpf, birthDate);
        BenefitRepository repo = (BenefitRepository)_BenefitRepository;
        await repo.Context.AddAsync(entity);

        var evt = _Mapper.Map<Beneficiary, BeneficiarySubmitted>(entity);
        evt.CorrelationId = _CorrerID.Value;
        await _Publisher.Publish(evt);
        await repo.Context.SaveChangesAsync();

        try
        {
        }
        catch (DbUpdateException exception)
        {
            throw new DuplicateRegistrationException("Duplicate registration", exception);
        }

        return entity;

        //catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        //{
        //    throw new DuplicateRegistrationException("Duplicate registration", exception);
        //}

        // return registration;


        //entity = await _BenefitRepository.AddAsync(entity);


        //await _Publisher.Publish(evt);

        //try
        //{
        //    await _dbContext.SaveChangesAsync();
        //}
        //catch (DbUpdateException exception)
        //{
        //    throw new DuplicateRegistrationException("Duplicate registration", exception);
        //}
        //catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        //{
        //    throw new DuplicateRegistrationException("Duplicate registration", exception);
        //}

        return entity;
    }
}