using System;
using System.Collections.Generic;

namespace Project_PRN222.Models;

public partial class ShipmentMethod
{
    public int ShipmentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Cost { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
