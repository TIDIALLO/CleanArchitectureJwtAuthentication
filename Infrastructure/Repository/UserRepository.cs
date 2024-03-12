using Application.Contracts;
using Application.DTOs;
using Application.Response;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Repository;

public class UserRepository : IUser
{
    private readonly AppDbContext _Dbcontext;
    private readonly IConfiguration _configuration;

    public UserRepository(AppDbContext dbcontext, IConfiguration configuration)
    {
        _Dbcontext = dbcontext;
        _configuration = configuration;
    }

    private async Task<User> FindUserByEmail(string email) =>
        await _Dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<LoginResponse> LoginUserAsync(LoginDto loginDto)
    {
        //var getUser = await _Dbcontext.Users.FirstOrDefaultAsync( u => u.Email == loginDto.Email);
        var getUser = await FindUserByEmail(loginDto.Email! );
        if (getUser == null)  return new LoginResponse(false, "User Not Found, sorry!!");
        
        bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
        if (checkPassword)
            return new LoginResponse(true, "Login Succcesfully", GenerateJwtToken(getUser));
        else return new LoginResponse(false, "Invalid credentials!!");

    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Name!),
                new Claim(ClaimTypes.Email,user.Email!)
        };
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RegistrationResponse> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var getUser = await FindUserByEmail(registerUserDto.Email!);
        if (getUser != null)
            return new RegistrationResponse(false, "User alredy exist!!");
        _Dbcontext.Users.Add(new User()
        {
            Name = registerUserDto.Name,
            Email = registerUserDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
        });
        await _Dbcontext.SaveChangesAsync();    
        return new RegistrationResponse(true, "registration Completed");

    }
}
