using MassTransit;
using Toolkit.Web;
using Toolkit.Mapper;
using Benefit.API.DTO;
using Toolkit.Interfaces;
using Toolkit.Exceptions;
using Benefit.Domain.Benefit;
using Benefit.Domain.Operator;
using Benefit.Domain.Interfaces;
using Toolkit.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Benefit.Service.Sagas.Beneficiary.Contract;

namespace Benefit.API.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BenefitApi : ManagedController
{
    public BenefitApi(IPublishEndpoint publisher, ITokenService tokenService, IBenefitRepository repository)
    {
        _Publisher = publisher;
        _TokenService = tokenService;
        _Repository = repository;
        _Mapper = MapperFactory.Nest<Beneficiary, BeneficiaryResponse>()
            .Nest<Beneficiary, BeneficiarySubmitted>()
            .Nest<TheAudioDbWork, BeneficiaryTheAudioDbWorkResponse>()
            .Build<ImdbWork, BeneficiaryImdbWorkResponse>();
    }

    private readonly IGenericMapper _Mapper;
    private readonly ITokenService _TokenService;
    private readonly IPublishEndpoint _Publisher;
    private readonly IBenefitRepository _Repository;

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
            return BadRequest("Invalid credentials.");
        var oktaToken = await _TokenService.GetToken(loginRequest.UserName, loginRequest.Password);
        if (oktaToken != null)
            return Ok(oktaToken);
        return null;
    }

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
            return await SubmitBeneficiary(beneficiary.Operator, beneficiary.Name, beneficiary.CPF, beneficiary.BirthDate);
        };
        Func<object, IActionResult> action = delegate (object result)
        {
            Beneficiary b = result as Beneficiary;
            return CreatedAtAction(nameof(GetBeneficiaryById).ToLower(), new { id = b.ID }, null);
        };
        return await TryExecute(action, execute);
    }

    private async Task<List<BeneficiaryResponse>> GetBeneficiaries(int? limit = 10, int? start = 0)
    {
        var resutlt = await _Repository.GetAsync(limit ?? 10, start ?? 0);
        return resutlt.Select(o => _Mapper.Map<Beneficiary, BeneficiaryResponse>(o)).ToList();
    }

    private async Task<BeneficiaryResponse> GetBeneficiaryByIdAsync(int id)
    {
        var beneficiary = await _Repository.GetObjectByIDAsync(id);
        return _Mapper.Map<Beneficiary, BeneficiaryResponse>(beneficiary);
    }

    private async Task<Beneficiary> SubmitBeneficiary(OperatorType operatorType, string name, string cpf, DateTime? birthDate)
    {
        try
        {
            var op = Operator.CreateOperator(operatorType);
            var entity = op.CreateBeneficiary(name, cpf, birthDate);
            if (await _Repository.GetByCPF(cpf) != null)
                throw new DomainRuleException($"There is already a beneficiary registered with the cpf \"{cpf}\".");
            await _Repository.AddAsync(entity, false);
            var evt = _Mapper.Map<Beneficiary, BeneficiarySubmitted>(entity);
            evt.CorrelationId = NewId.NextGuid();
            await _Publisher.Publish(evt);
            await _Repository.SaveChangesAsync();
            return entity;
        }
        catch (DbUpdateException exception)
        {
            throw new DuplicateRegistrationException("Duplicate registration", exception);
        }
    }
}