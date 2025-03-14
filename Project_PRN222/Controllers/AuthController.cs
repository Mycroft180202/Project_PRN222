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

        [HttpGet("register")] // Action to return Register View
        public IActionResult Register()
        {
            return View("Register"); // Assuming you have a Register.cshtml view in Views/Auth
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

        [HttpGet("login")] // Action to return Login View
        public IActionResult Login()
        {
            return View("Login"); // Assuming you have a Login.cshtml view in Views/Auth
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.Login(loginDto);
                // Redirect based on role after login
                if (user.RoleId == 1)
                {
                    return RedirectToAction("AdminDashboard", "Home"); // Redirect to Admin Dashboard
                }
                else if (user.RoleId == 2)
                {
                    return RedirectToAction("VendorDashboard", "Home"); // Redirect to Vendor Dashboard
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirect to Home page for regular users
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("forgot-password")] // Action to return ForgotPassword View
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword"); // Assuming you have a ForgotPassword.cshtml view in Views/Auth
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

        [HttpGet("reset-password")] // Action to return ResetPassword View
        public IActionResult ResetPassword()
        {
            return View("ResetPassword"); // Assuming you have a ResetPassword.cshtml view in Views/Auth
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
