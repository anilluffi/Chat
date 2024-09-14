using AutoMapper;
using MessangerBackend.Core.Interfaces;
using MessangerBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessangerBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : Controller
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IMapper mapper, IUserService userService, IConfiguration configuration)
    {
        _mapper = mapper;
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] int page, [FromQuery] int size)
    {
        var users = _userService.GetUsers(page, size);
        return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDTO>> GetUserById(int id)
    {
        return Ok(_mapper.Map<UserDTO>(await _userService.GetUserById(id)));
    }

    [HttpGet("search/{name}")]
    public ActionResult<IEnumerable<UserDTO>> SearchUsers(string name)
    {
        var users = _userService.SearchUsers(name);
        return Ok(_mapper.Map<IEnumerable<UserDTO>>(users));
    }
}