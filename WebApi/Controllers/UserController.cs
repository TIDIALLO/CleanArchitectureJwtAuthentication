using Application.Contracts;
using Application.DTOs;
using Application.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUser _user;
    public UserController(IUser user)
    {
        _user = user;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> LoginUser(LoginDto loginDto)
    {
        var result = await _user.LoginUserAsync(loginDto);
        return Ok(result);
    }   
    
    [HttpPost("register")]
    public async Task<ActionResult<RegistrationResponse>> RegisterUser(RegisterUserDto registerUserDto)
    {
        var result = await _user.RegisterAsync(registerUserDto);
        return Ok(result);
    }

}
