﻿using System;
using System.Collections.Generic;

namespace Project_PRN222.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string ProductName { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
