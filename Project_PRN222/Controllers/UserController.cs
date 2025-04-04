﻿using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Attributes;
using Project_PRN222.DTO;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IVendorService _vendorService;

        public UserController(IUserService userService, IVendorService vendorService)
        {
            _userService = userService;
            _vendorService = vendorService;
        }

        // Action to return User Profile View (for logged in user)
        [HttpGet("Profile")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> ProfileView()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return Unauthorized("User not logged in.");
            }

            var user = await _userService.GetById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View("Profile", user); // Assuming you have a Profile.cshtml in Views/User, passing the user model
        }


        [HttpPut("me")]
        [RoleAuthorize(1, 2, 3)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserDto updateDto)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return Unauthorized("User not logged in.");
            }

            var user = await _userService.GetById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Email = updateDto.Email ?? user.Email;
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = updateDto.Address ?? user.Address;

            await _userService.Update(user);

            // Nếu là Vendor, cập nhật thông tin trong bảng Vendor
            if (user.RoleId == 2)
            {
                var vendor = await _vendorService.GetByUserId(userId);
                if (vendor != null)
                {
                    vendor.ContactEmail = user.Email;
                    vendor.ContactPhone = user.PhoneNumber;
                    vendor.Address = user.Address;
                    await _vendorService.UpdateVendor(vendor);
                }
            }

            return NoContent();
        }

        // Action to return Edit User Role View (Admin only)
        [HttpGet("EditRoleView/{id}")]
        [RoleAuthorize(1)]
        public async Task<IActionResult> EditRoleView(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View("EditRole", user); // Assuming you have an EditRole.cshtml in Views/User, passing the user model
        }


        // Thay đổi IsActive của tài khoản
        [HttpPut("{id}/active")]
        [RoleAuthorize(1)]
        public async Task<IActionResult> UpdateUserActive(int id, [FromBody] bool isActive)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.IsActive = isActive;
            await _userService.Update(user);
            return NoContent();
        }

        // Thay đổi RoleId của tài khoản và lưu vào Vendor nếu RoleId = 2
        [HttpPut("{id}/role")]
        [RoleAuthorize(1)]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] int newRoleId)
        {
            if (newRoleId < 1 || newRoleId > 3)
            {
                return BadRequest("Invalid RoleId. Allowed values are 1 (Admin), 2 (Vendor), 3 (User).");
            }

            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.RoleId = newRoleId;
            await _userService.Update(user);

            // Nếu RoleId được đổi thành 2 (Vendor), lưu vào bảng Vendor
            if (newRoleId == 2)
            {
                var existingVendor = await _vendorService.GetByUserId(id);
                if (existingVendor == null)
                {
                    var vendor = new Vendor
                    {
                        VendorName = user.UserName,
                        ContactEmail = user.Email,
                        ContactPhone = user.PhoneNumber,
                        Address = user.Address,
                        CreatedDate = DateTime.Now,
                        UserId = user.UserId,
                        IsActive = true
                    };
                    await _vendorService.CreateVendor(vendor);
                }
            }

            return NoContent();
        }

        // Action to return User List View (Admin only)
        [HttpGet("ListView")]
        [RoleAuthorize(1)] // Chỉ Admin
        public async Task<IActionResult> ListView()
        {
            var users = await _userService.GetAll();
            return View("List", users); // Assuming you have a List.cshtml in Views/User to display list of users, passing users model
        }


        // Xem thông tin tất cả người dùng (cho Admin)
        [HttpGet]
        [RoleAuthorize(1)] // Chỉ Admin
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        // Action to return User Details View (Admin only)
        [HttpGet("DetailsView/{id}")]
        [RoleAuthorize(1)]
        public async Task<IActionResult> DetailsView(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return View("Details", user); // Assuming you have a Details.cshtml in Views/User, passing the user model
        }

        // Xem thông tin một người dùng
        [HttpGet("{id}")]
        [RoleAuthorize(1)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }
    }
}