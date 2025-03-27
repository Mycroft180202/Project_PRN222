using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project_PRN222.DTO;
using Project_PRN222.Hubs;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AuthController(IAuthService authService, IHubContext<NotificationHub> hubContext)
        {
            _authService = authService;
            _hubContext = hubContext;
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
                if (!ModelState.IsValid)
                {
                    return View("Register", registerDto); // Trả về View với dữ liệu đã nhập nếu validation thất bại
                }

                var user = await _authService.Register(registerDto);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Registration successful");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Lưu lỗi vào TempData
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", $"Error: {ex.Message}");
                return View("Register", registerDto); // Trả về View với dữ liệu đã nhập
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
                if (!ModelState.IsValid)
                {
                    return View("Login", loginDto); // Trả về View nếu validation thất bại
                }

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
                TempData["Error"] = ex.Message; // Lưu lỗi vào TempData
                return View("Login", loginDto); // Trả về View với dữ liệu đã nhập
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
                if (!ModelState.IsValid)
                {
                    return View("ForgotPassword", forgotPasswordDto); // Trả về View nếu validation thất bại
                }

                await _authService.GeneratePasswordResetCode(forgotPasswordDto.Email);
                TempData["Success"] = "Reset code sent to email"; // Thông báo thành công
                return View("ForgotPassword"); // Trả về View
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Lưu lỗi vào TempData
                return View("ForgotPassword", forgotPasswordDto); // Trả về View với dữ liệu đã nhập
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
                if (!ModelState.IsValid)
                {
                    return View("ResetPassword", resetDto); // Trả về View nếu validation thất bại
                }

                var result = await _authService.ResetPassword(resetDto);
                TempData["Success"] = "Password reset successful"; // Thông báo thành công
                return RedirectToAction("Login"); // Chuyển hướng về Login sau khi reset thành công
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Lưu lỗi vào TempData
                return View("ResetPassword", resetDto); // Trả về View với dữ liệu đã nhập
            }
        }

        // --- Các endpoint API trả về JSON cho React (giữ nguyên) ---

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
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            
            HttpContext.Session.Clear();
    
            
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
    
            
            return RedirectToAction("Index", "Home");
        }
    }
}