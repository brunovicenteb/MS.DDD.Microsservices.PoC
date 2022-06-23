using Toolkit.Web;
using Benefit.Domain.BeneficiaryAggregate;
using System.Collections.Concurrent;
using Benefit.Domain.Operator;
using Toolkit.Exceptions;
using Benefit.API.ValueObject;

namespace MS.DDD.Microsservices.PoC.Benefit.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BenefitApi : ManagedController
{
    private static readonly ConcurrentDictionary<uint, Beneficiary> _Cache = new ConcurrentDictionary<uint, Beneficiary>();

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BeneficiaryOutputVO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(uint id)
    {
        return await TryExecuteOK(async () => await GetBeneficiaryById(id));
    }

    [HttpGet("getall")]
    [ProducesResponseType(typeof(List<BeneficiaryOutputVO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return await TryExecuteOK(async () => await GetAllBeneficiary());
    }

    [HttpPost]
    [ProducesResponseType(typeof(BeneficiaryOutputVO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBenefit([FromBody] BeneficiaryInputVO beneficiary)
    {
        Func<object, IActionResult> action = delegate (object result)
        {
            BeneficiaryOutputVO c = result as BeneficiaryOutputVO;
            return CreatedAtAction(nameof(Get).ToLower(), new { id = c.ID }, result);
        };
        return await TryExecute(action, async () => await Create(beneficiary));
    }

    private async Task<List<BeneficiaryOutputVO>> GetAllBeneficiary()
    {
        await Task.CompletedTask;
        return _Cache.Values.Select(o => new BeneficiaryOutputVO(o)).ToList();
    }

    private async Task<BeneficiaryOutputVO> GetBeneficiaryById(uint id)
    {
        await Task.CompletedTask;
        if (_Cache.TryGetValue(id, out Beneficiary beneficiary))
            return new BeneficiaryOutputVO(beneficiary);
        throw new NotFoundException("Beneficiary not found.");
    }

    private async Task<BeneficiaryOutputVO> Create(BeneficiaryInputVO beneficiaryVO)
    {
        await Task.CompletedTask;
        if (beneficiaryVO == null)
            return null;
        BaseOperator op = OperatorFactory.CreateOperator(beneficiaryVO.Operator);
        Beneficiary beneficiary = op.CreateBeneficiary(beneficiaryVO.ParentID, beneficiaryVO.Name, beneficiaryVO.CPF, beneficiaryVO.BirthDate);
        _Cache.TryAdd(beneficiary.ID, beneficiary);
        return new BeneficiaryOutputVO(beneficiary);
    }
}