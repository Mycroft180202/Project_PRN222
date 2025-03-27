using System;
using System.Collections.Generic;

namespace Project_PRN222.Models;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public int? OrderId { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? DeliveryStatus { get; set; }

    public string? TrackingCode { get; set; }

    public int? ShipmentMethodId { get; set; }

    public string? ShippingAddress { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ShipmentMethod? ShipmentMethod { get; set; }
}
