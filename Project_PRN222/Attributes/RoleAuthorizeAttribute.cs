using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;
using System.Text.Json;

namespace Project_PRN222.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizeAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var roleId = session.GetString("RoleId");
            var isActive = session.GetString("IsActive"); // Lấy IsActive từ Session

            // Kiểm tra người dùng đã đăng nhập và kích hoạt chưa
            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(isActive) || !bool.Parse(isActive))
            {
                context.Result = new ForbidResult(); // Chặn nếu chưa active
                return;
            }

            if (!_allowedRoles.Contains(int.Parse(roleId)))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (_allowedRoles.Contains(2) && int.Parse(roleId) == 2) // Vendor
            {
                var userId = int.Parse(session.GetString("UserId"));
                var request = context.HttpContext.Request;
                var productService = context.HttpContext.RequestServices.GetService<IProductService>();
                var vendorService = context.HttpContext.RequestServices.GetService<IVendorService>();

                if (request.Path.Value.Contains("ProductController"))
                {
                    if (request.Method == "PUT" || request.Method == "DELETE") // Update/Delete
                    {
                        var id = int.Parse(context.HttpContext.Request.RouteValues["id"]?.ToString() ?? "0");
                        if (id > 0)
                        {
                            var product = productService.GetProductById(id);
                            if (product != null && product.VendorId != null)
                            {
                                var vendor = vendorService.GetByUserId(userId).Result;
                                if (vendor == null || product.VendorId != vendor.VendorId)
                                {
                                    context.Result = new ForbidResult();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}