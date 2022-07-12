using MassTransit;
using Toolkit.Web;
using Benefit.API.DTO;
using Toolkit.Interfaces;
using Benefit.Domain.Events;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Toolkit.Mapper;
using Benefit.Domain.Operator;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    public BenefitApi(IPublishEndpoint publiser, IBenefitRepository benefitRepository)
    {
        _Publisher = publiser;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Nest<Beneficiary, BeneficiaryResponse>()
            .Nest<Beneficiary, BenefitCreatedEvent>()
            .Nest<Work, BeneficiaryWorkResponse>()
            .Build<BeneficiaryCreateRequest, BenefitInsertedEvent>();
    }

    private readonly IGenericMapper _Mapper;
    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _BenefitRepository;

    /// <summary>Returns the registered benefits with the possibility of pagination.</summary>
    /// <param name="limit">Maximum number of results possible.</param>
    /// <param name="start">Skip a specific number of entries. This feature is especially useful for pagination.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<BeneficiaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int? limit = 10, int? start = 0)
    {
        return await TryExecuteOK(async () => await GetBeneficiaries(limit, start));
    }

    /// <summary>Returns an benefit by identifier.</summary>
    /// <param name="id">Identifier of benefit.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(int id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    /// <summary>Create a new benefit.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBenefit([FromBody] BeneficiaryCreateRequest beneficiary)
    {
        Func<Task<object>> execute = async delegate
        {
            await Task.CompletedTask;
            var op = Operator.CreateOperator(beneficiary.Operator);
            var entity = op.CreateBeneficiary(beneficiary.Name, beneficiary.CPF, beneficiary.BirthDate);
            return _BenefitRepository.Add(entity);
        };
        Func<object, IActionResult> action = delegate (object result)
        {
            Beneficiary b = result as Beneficiary;
            BenefitCreatedEvent evt = _Mapper.Map<Beneficiary, BenefitCreatedEvent>(b);
            _Publisher.Publish(evt);
            return CreatedAtAction(nameof(GetBeneficiaryById).ToLower(), new { id = b.ID });
        };
        return await TryExecute(action, execute);
    }

    private async Task<List<BeneficiaryResponse>> GetBeneficiaries(int? limit = 10, int? start = 0)
    {
        await Task.CompletedTask;
        return _BenefitRepository.Get(limit ?? 10, start ?? 0)
            .Select(o => _Mapper.Map<Beneficiary, BeneficiaryResponse>(o)).ToList();
    }

    private async Task<BeneficiaryResponse> GetBeneficiaryById(int id)
    {
        await Task.CompletedTask;
        var beneficiary = _BenefitRepository.GetObjectByID(id);
        return _Mapper.Map<Beneficiary, BeneficiaryResponse>(beneficiary);
    }
}