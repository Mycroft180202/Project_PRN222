using Project_PRN222.DTO;
using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> Register(RegisterDto registerDto);
        Task<User> Login(LoginDto loginDto);
        Task<string> GeneratePasswordResetCode(string email);
        Task<bool> ResetPassword(ResetPasswordDto resetDto);
    }
}
