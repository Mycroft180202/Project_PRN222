using Project_PRN222.Models;

namespace Project_PRN222.DTO;

public class OrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
    public string ShippingAddress { get; set; }
    public string BillingAddress { get; set; }
    public User User { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}