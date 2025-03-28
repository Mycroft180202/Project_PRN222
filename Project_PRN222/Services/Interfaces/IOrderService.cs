using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Models;

namespace Project_PRN222.Services.Interfaces
{
    public interface IOrderService
    {
        Task<string> Checkout(string shippingAddress, string billingAddress, int shipmentMethodId, string paymentMethod);
        Task<IActionResult> ProcessVnpayCallback(IQueryCollection query);
        Task<Order> GetOrderById(int orderId);
        Task<List<Order>> GetOrdersWithDetails();
    }
}