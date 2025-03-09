using System;
using System.Collections.Generic;

namespace Project_PRN222.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string? ShippingAddress { get; set; }

    public string? BillingAddress { get; set; }

    public string? PaymentMethod { get; set; }

    public int? ShipmentMethodId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Delivery? Delivery { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ShipmentMethod? ShipmentMethod { get; set; }

    public virtual User? User { get; set; }
}
