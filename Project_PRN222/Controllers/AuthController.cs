using Microsoft.AspNetCore.Mvc;
using Project_PRN222.DTO;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.Register(registerDto);
                return Ok(new { Message = "Registration successful", UserId = user.UserId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.Login(loginDto);
                return Ok(new
                {
                    Message = "Login successful",
                    UserId = user.UserId,
                    RoleId = user.RoleId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                await _authService.GeneratePasswordResetCode(forgotPasswordDto.Email);
                return Ok(new { Message = "Reset code sent to email" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                var result = await _authService.ResetPassword(resetDto);
                return Ok(new { Message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
