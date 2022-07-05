using MassTransit;
using Toolkit.Web;
using Benefit.API.DTO;
using Toolkit.Exceptions;
using System.Collections.Concurrent;
using Benefit.Domain.Benefit;
using Toolkit.Configurations;
using Benefit.Domain.Events;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    readonly IBus _Bus;
    private readonly GenericMapper _Mapper;
    private static readonly ConcurrentDictionary<uint, Beneficiary> _Cache = new ConcurrentDictionary<uint, Beneficiary>();
    public BenefitApi(GenericMapper mapper, IBus bus)
    {
        _Mapper = mapper;
        _Bus = bus;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(uint id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(List<BeneficiaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return await TryExecuteOK(async () => await GetAllBeneficiary());
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBenefit([FromBody] BeneficiaryCreateRequest beneficiary)
    {
        var beneficiaryEvt = _Mapper.Map<BeneficiaryCreateRequest, BenefitInsertedEvent>(beneficiary);
        await _Bus.Publish(beneficiaryEvt);
        return Accepted();
    }

    private async Task<List<BeneficiaryCreateRequest>> GetAllBeneficiary()
    {
        await Task.CompletedTask;
        return _Cache.Values.Select(o => _Mapper.Map<Beneficiary, BeneficiaryCreateRequest>(o)).ToList();
    }

    private async Task<BeneficiaryCreateRequest> GetBeneficiaryById(uint id)
    {
        await Task.CompletedTask;
        if (_Cache.TryGetValue(id, out Beneficiary beneficiary))
            return _Mapper.Map<Beneficiary, BeneficiaryCreateRequest>(beneficiary);
        throw new NotFoundException("Beneficiary not found.");
    }
}