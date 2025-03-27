using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Project_PRN222.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? AddedDate { get; set; }

    [JsonIgnore]
    public virtual Product? Product { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}
