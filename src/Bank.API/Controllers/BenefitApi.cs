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
            cfg.CreateMap<BeneficiaryInDTO, Beneficiary>();
            cfg.CreateMap<Beneficiary, BeneficiaryOutDTO>();
        });
        _Mapper = new Mapper(configuration);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryOutDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(uint id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(List<BeneficiaryOutDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return await TryExecuteOK(async () => await GetAllBeneficiary());
    }

    [HttpPost]
    [ProducesResponseType(typeof(BeneficiaryOutDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBenefit([FromBody] BeneficiaryInDTO beneficiary)
    {
        Func<object, IActionResult> action = delegate (object result)
        {
            BeneficiaryOutDTO c = result as BeneficiaryOutDTO;
            return CreatedAtAction(nameof(Get).ToLower(), new { id = c.ID }, result);
        };
        return await TryExecute(action, async () => await Create(beneficiary));
    }

    private async Task<List<BeneficiaryOutDTO>> GetAllBeneficiary()
    {
        await Task.CompletedTask;
        return _Cache.Values.Select(o => _Mapper.Map<BeneficiaryOutDTO>(o)).ToList();
    }

    private async Task<BeneficiaryOutDTO> GetBeneficiaryById(uint id)
    {
        await Task.CompletedTask;
        if (_Cache.TryGetValue(id, out Beneficiary beneficiary))
            return _Mapper.Map<BeneficiaryOutDTO>(beneficiary);
        throw new NotFoundException("Beneficiary not found.");
    }

    private async Task<BeneficiaryOutDTO> Create(BeneficiaryInDTO beneficiaryVO)
    {
        await Task.CompletedTask;
        if (beneficiaryVO == null)
            return null;
        BaseOperator op = OperatorFactory.CreateOperator(beneficiaryVO.Operator);
        Beneficiary beneficiary = op.CreateBeneficiary(beneficiaryVO.ParentID, beneficiaryVO.Name, beneficiaryVO.CPF, beneficiaryVO.BirthDate);
        _Cache.TryAdd(beneficiary.ID, beneficiary);
        return _Mapper.Map<BeneficiaryOutDTO>(beneficiary);
    }
}