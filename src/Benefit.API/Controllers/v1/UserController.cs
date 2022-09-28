using Toolkit.Web;
using System.Security.Claims;
using Toolkit.Identity.Interfaces;
using Toolkit.Identity.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Toolkit.Identity.DTOs.Response;

namespace Way2Commerce.Api.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : ManagedController
{
    private IIdentityService _IdentityService;

    public UserController(IIdentityService identityService) =>
        _IdentityService = identityService;

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(UserCreateRequest user)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var result = await _IdentityService.CreateUser(user);
        if (result.Sucess)
            return Ok(result);
        else if (result.Errors.Count > 0)
            return BadRequest(result);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponse>> Login(UserLoginRequest userLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest();
        var result = await _IdentityService.Login(userLogin);
        if (result.Sucess)
            return Ok(result);
        return Unauthorized(result);
    }
}