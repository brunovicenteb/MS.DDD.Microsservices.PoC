using AutoMapper;
using Toolkit.Web;
using Benefit.API.DTO;
using Toolkit.Exceptions;
using Benefit.Domain.Operator;
using System.Collections.Concurrent;
using Benefit.Domain.Benefit;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    private static readonly Mapper _Mapper;
    private static readonly ConcurrentDictionary<uint, Beneficiary> _Cache = new ConcurrentDictionary<uint, Beneficiary>();
    static BenefitApi()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<BeneficiaryResponse, Beneficiary>();
            cfg.CreateMap<Beneficiary, BeneficiaryCreateRequest>();
        });
        _Mapper = new Mapper(configuration);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryCreateRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(uint id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(List<BeneficiaryCreateRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return await TryExecuteOK(async () => await GetAllBeneficiary());
    }

    [HttpPost]
    [ProducesResponseType(typeof(BeneficiaryCreateRequest), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBenefit([FromBody] BeneficiaryResponse beneficiary)
    {
        Func<object, IActionResult> action = delegate (object result)
        {
            BeneficiaryCreateRequest c = result as BeneficiaryCreateRequest;
            return CreatedAtAction(nameof(Get).ToLower(), new { id = c.ID }, result);
        };
        return await TryExecute(action, async () => await Create(beneficiary));
    }

    private async Task<List<BeneficiaryCreateRequest>> GetAllBeneficiary()
    {
        await Task.CompletedTask;
        return _Cache.Values.Select(o => _Mapper.Map<BeneficiaryCreateRequest>(o)).ToList();
    }

    private async Task<BeneficiaryCreateRequest> GetBeneficiaryById(uint id)
    {
        await Task.CompletedTask;
        if (_Cache.TryGetValue(id, out Beneficiary beneficiary))
            return _Mapper.Map<BeneficiaryCreateRequest>(beneficiary);
        throw new NotFoundException("Beneficiary not found.");
    }

    private async Task<BeneficiaryCreateRequest> Create(BeneficiaryResponse beneficiaryVO)
    {
        await Task.CompletedTask;
        if (beneficiaryVO == null)
            return null;
        BaseOperator op = OperatorFactory.CreateOperator(beneficiaryVO.Operator);
        Beneficiary beneficiary = op.CreateBeneficiary(beneficiaryVO.ParentID, beneficiaryVO.Name, beneficiaryVO.CPF, beneficiaryVO.BirthDate);
        _Cache.TryAdd(beneficiary.ID, beneficiary);
        return _Mapper.Map<BeneficiaryCreateRequest>(beneficiary);
    }
}