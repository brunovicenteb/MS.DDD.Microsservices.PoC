using MassTransit;
using Toolkit.Web;
using Toolkit.Mapper;
using Benefit.API.DTO;
using Toolkit.Interfaces;
using Benefit.Domain.Benefit;
using Benefit.Domain.Interfaces;
using Benefit.Service.Interfaces;
using Benefit.Service.Sagas.Beneficiary.Contract;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    public BenefitApi(IBeneficiaryService service, IBenefitRepository benefitRepository)
    {
        _Service = service;
        _BenefitRepository = benefitRepository;
        _Mapper = MapperFactory.Nest<Beneficiary, BeneficiaryResponse>()
            .Nest<Beneficiary, BeneficiarySubmitted>()
            .Nest<TheAudioDbWork, BeneficiaryTheAudioDbWorkResponse>()
            .Build<ImdbWork, BeneficiaryImdbWorkResponse>();
    }

    private readonly IGenericMapper _Mapper;
    private readonly IBeneficiaryService _Service;
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
    [HttpGet("GetBeneficiaryById/{id}")]
    [ProducesResponseType(typeof(BeneficiaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBeneficiaryById(int id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryByIdAsync(id));
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
            return await _Service.SubmitBeneficiary(beneficiary.Operator, beneficiary.Name, beneficiary.CPF, beneficiary.BirthDate);
        };
        Func<object, IActionResult> action = delegate (object result)
        {
            Beneficiary b = result as Beneficiary;
            return CreatedAtAction(nameof(GetBeneficiaryById).ToLower(), new { id = b.ID });
        };
        return await TryExecute(action, execute);
    }

    private async Task<List<BeneficiaryResponse>> GetBeneficiaries(int? limit = 10, int? start = 0)
    {
        var resutlt = await _BenefitRepository.GetAsync(limit ?? 10, start ?? 0);
        return resutlt.Select(o => _Mapper.Map<Beneficiary, BeneficiaryResponse>(o)).ToList();
    }

    private async Task<BeneficiaryResponse> GetBeneficiaryByIdAsync(int id)
    {
        var beneficiary = await _BenefitRepository.GetObjectByIDAsync(id);
        return _Mapper.Map<Beneficiary, BeneficiaryResponse>(beneficiary);
    }
}