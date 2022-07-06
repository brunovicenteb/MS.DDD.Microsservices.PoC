using AutoMapper;
using MassTransit;
using Toolkit.Web;
using Benefit.API.DTO;
using Benefit.Domain.Benefit;
using Toolkit.Configurations;
using Benefit.Domain.Events;
using Benefit.Domain.Interfaces;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    public BenefitApi(IBus bus, GenericMapper mapper, IBenefitRepository benefitRepository)
    {
        _Bus = bus;
        _Mapper = mapper;
        _BenefitRepository = benefitRepository;
        _ResponseMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Beneficiary, BeneficiaryResponse>();
            cfg.CreateMap<Work, BeneficiaryWorkResponse>();
        });
        _ResponseMapperConfig.AssertConfigurationIsValid();
    }

    private readonly IBus _Bus;
    private readonly GenericMapper _Mapper;
    private readonly IBenefitRepository _BenefitRepository;
    private readonly MapperConfiguration _ResponseMapperConfig;

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
    public async Task<IActionResult> Get(string id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    /// <summary>Create a new benefit.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<AcceptedResult> CreateBenefit([FromBody] BeneficiaryCreateRequest beneficiary)
    {
        var beneficiaryEvt = _Mapper.Map<BeneficiaryCreateRequest, BenefitInsertedEvent>(beneficiary);
        await _Bus.Publish(beneficiaryEvt);
        return Accepted();
    }

    private async Task<List<BeneficiaryResponse>> GetBeneficiaries(int? limit = 10, int? start = 0)
    {
        await Task.CompletedTask;
        var mapper = _ResponseMapperConfig.CreateMapper();
        return _BenefitRepository.Get(limit ?? 10, start ?? 0)
            .Select(o => mapper.Map<Beneficiary, BeneficiaryResponse>(o)).ToList();
    }

    private async Task<BeneficiaryResponse> GetBeneficiaryById(string id)
    {
        await Task.CompletedTask;
        var beneficiary = _BenefitRepository.GetObjectByID(id);
        return _Mapper.Map<Beneficiary, BeneficiaryResponse>(beneficiary);
    }
}