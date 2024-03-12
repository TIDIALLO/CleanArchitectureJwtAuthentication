

using Application.DTOs;
using Application.Response;

namespace Application.Contracts;

public interface IUser
{
    Task<RegistrationResponse> RegisterAsync(RegisterUserDto registerDto);
    Task<LoginResponse> LoginUserAsync(LoginDto loginDto);
}
