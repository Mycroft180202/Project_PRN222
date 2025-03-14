using Microsoft.AspNetCore.Mvc;
using Project_PRN222.DTO;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // --- Các endpoint trả về View cho ASP.NET MVC ---

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("Register"); // Trả về Register.cshtml
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
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

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login"); // Trả về Login.cshtml
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.Login(loginDto);
                if (user.RoleId == 1)
                {
                    return RedirectToAction("AdminDashboard", "Home");
                }
                else if (user.RoleId == 2)
                {
                    return RedirectToAction("VendorDashboard", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword"); // Trả về ForgotPassword.cshtml
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
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

        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            return View("ResetPassword"); // Trả về ResetPassword.cshtml
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetDto)
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

        // --- Các endpoint API trả về JSON cho React ---

        [HttpPost("api/register")]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterDto registerDto)
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

        [HttpPost("api/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.Login(loginDto);
                // Trả về JSON thay vì redirect
                return Ok(new
                {
                    Message = "Login successful",
                    UserId = user.UserId,
                    RoleId = user.RoleId
                    // Nếu dùng JWT, bạn có thể thêm accessToken và refreshToken ở đây
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("api/forgot-password")]
        public async Task<IActionResult> ApiForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
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

        [HttpPost("api/reset-password")]
        public async Task<IActionResult> ApiResetPassword([FromBody] ResetPasswordDto resetDto)
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