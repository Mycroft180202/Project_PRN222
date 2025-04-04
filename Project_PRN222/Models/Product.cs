﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Project_PRN222.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public int? VendorId { get; set; } 

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual Vendor? Vendor { get; set; } // Thay User bằng Vendor

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<SalesStatistic> SalesStatistics { get; set; } = new List<SalesStatistic>();
}
