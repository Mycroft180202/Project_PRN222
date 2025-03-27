using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Project_PRN222.DTO;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService; 

        public AuthService(IUserRepository userRepository, 
                            IVendorRepository vendorRepository, 
                            IHttpContextAccessor httpContextAccessor, 
                            IEmailService emailService)
        {
            _userRepository = userRepository;
            _vendorRepository = vendorRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<User> Register(RegisterDto registerDto)
        {
            if (await _userRepository.GetByEmail(registerDto.Email) != null)
                throw new Exception("Email already exists");

            if (await _userRepository.GetByUsername(registerDto.UserName) != null)
                throw new Exception("Username already exists");

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                RegistrationDate = DateTime.Now,
                IsActive = true,
                RoleId = 3 // Default là User
            };

            await _userRepository.Add(user);

            // Nếu là Vendor (RoleId = 2), thêm vào bảng Vendor
            if (user.RoleId == 2)
            {
                var vendor = new Vendor
                {
                    VendorName = user.UserName,
                    ContactEmail = user.Email,
                    ContactPhone = user.PhoneNumber,
                    Address = user.Address,
                    CreatedDate = DateTime.Now,
                    UserId = user.UserId
                };
                await _vendorRepository.Add(vendor);
            }

            return user;
        }

        public async Task<User> Login(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmail(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            if (!user.IsActive.GetValueOrDefault())
                throw new Exception("Account is not active.");

            // Lưu thông tin user vào Session
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("UserId", user.UserId.ToString());
            session.SetString("UserName", user.UserName);
            session.SetString("RoleId", user.RoleId.ToString());
            session.SetString("IsActive", user.IsActive.ToString());

            // Thiết lập Cookie Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
               
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return user;
        }

        public async Task<string> GeneratePasswordResetCode(string email)
        {
            var user = await _userRepository.GetByEmail(email);
            if (user == null)
                throw new Exception("Email not found");

            var resetCode = Guid.NewGuid().ToString("N").Substring(0, 6); // Tạo code 6 ký tự
            user.PasswordResetCode = resetCode;
            user.ResetCodeExpiry = DateTime.Now.AddHours(1); // Code hết hạn sau 1 giờ
            await _userRepository.Update(user);

            // Gửi email
            await _emailService.SendEmailAsync(email, "Password Reset Code",
                $"Your password reset code is: {resetCode}");

            return resetCode;
        }

        public async Task<bool> ResetPassword(ResetPasswordDto resetDto)
        {
            var user = await _userRepository.GetByEmail(resetDto.Email);
            if (user == null || user.PasswordResetCode != resetDto.ResetCode ||
                user.ResetCodeExpiry < DateTime.Now)
                throw new Exception("Invalid or expired reset code");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
            user.PasswordResetCode = null;
            user.ResetCodeExpiry = null;
            await _userRepository.Update(user);
            return true;
        }
    }
}
