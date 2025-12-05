using BusTrackBackEnd.API.IAM.Domain.Services;
using BusTrackBackEnd.API.IAM.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;

namespace BusTrackBackEnd.API.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserCommandService _commandService;

    public AuthenticationController(IUserCommandService commandService)
    {
        _commandService = commandService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> Register([FromBody] SignUpResource body)
    {
        await _commandService.RegisterAsync(new(body.Username, body.Email, body.Password));
        return Ok();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource body)
    {
        var token = await _commandService.SignInAsync(new(body.Username, body.Password));
        return Ok(new { token });
    }
}