using AutoMapper;
using Toolkit.Web;
using Benefit.API.DTO;
using Toolkit.Exceptions;
using Benefit.Domain.Operator;
using System.Collections.Concurrent;
using Benefit.Domain.AggregatesModel.BeneficiaryAggregate;
using Benefit.API.DTO.Beneficiary;
using Benefit.Domain.AggregatesModel.BeneficiaryAggregate.Rules;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BeneficiaryController : ManagedController
{
    private readonly IMapper _Mapper;
    private readonly ConcurrentDictionary<uint, Beneficiary> _Cache = new ConcurrentDictionary<uint, Beneficiary>();
    public BeneficiaryController(IMapper mapper)
    {
        _Mapper = mapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(uint id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(List<BeneficiaryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return await TryExecuteOK(async () => await GetAllBeneficiary());
    }

    [HttpPost]
    [ProducesResponseType(typeof(BeneficiaryOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBeneficiary([FromBody] BeneficiaryInput beneficiary)
    {
        Func<object, IActionResult> action = delegate (object result)
        {
            BeneficiaryOutput c = result as BeneficiaryOutput;
            return CreatedAtAction(nameof(Get).ToLower(), new { id = c.ID }, result);
        };
        return await TryExecute(action, async () => await Create(beneficiary));
    }

    private async Task<List<BeneficiaryOutput>> GetAllBeneficiary()
    {
        await Task.CompletedTask;
        return _Cache.Values.Select(o => _Mapper.Map<BeneficiaryOutput>(o)).ToList();
    }

    private async Task<BeneficiaryOutput> GetBeneficiaryById(uint id)
    {
        await Task.CompletedTask;
        if (_Cache.TryGetValue(id, out Beneficiary beneficiary))
            return _Mapper.Map<BeneficiaryOutput>(beneficiary);
        throw new NotFoundException("Beneficiary not found.");
    }

    private async Task<BeneficiaryOutput> Create(BeneficiaryInput beneficiaryDTO)
    {
        //await Task.CompletedTask;

        if (beneficiaryDTO == null)
            throw new BadRequestException("invalid input object");

        var beneficiary = _Mapper.Map<Beneficiary>(beneficiaryDTO);
        var result = new BeneficiaryValidator().Validate(beneficiary);

        if (!result.IsValid)
            throw new BadRequestException(string.Join("", result.Errors.SelectMany(s => s.ErrorMessage)));

        BaseOperator op = OperatorFactory.CreateOperator(beneficiaryDTO.Operator);
        op.CreateBeneficiary(beneficiary);

        //Beneficiary beneficiary = new Beneficiary(beneficiary.ParentID, beneficiary.Name, beneficiary.CPF, beneficiary.BirthDate);
        //_Cache.TryAdd(beneficiary.ID, beneficiary);

        return _Mapper.Map<BeneficiaryOutput>(beneficiary);
    }
}