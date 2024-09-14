using MessangerBackend.Core.Interfaces;
using MessangerBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MessangerBackend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> RegisterUser(CreateUserRequest request)
    {
        var userDb = await _userService.Register(request.Nickname, request.Password);
        var jwt = JwtGenerator.GenerateJwt(userDb, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));
        
        HttpContext.Session.SetInt32("id", userDb.Id);

        return Created("token", jwt);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(CreateUserRequest request)
    {
        var user = await _userService.Login(request.Nickname, request.Password);
        var jwt = JwtGenerator.GenerateJwt(user, _configuration.GetValue<string>("TokenKey")!, DateTime.UtcNow.AddMinutes(5));

        return Created("token", jwt);
    }
}