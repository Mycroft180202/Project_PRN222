using System;
using System.Collections.Generic;

namespace Project_PRN222.Models;

public partial class SalesStatistic
{
    public int StatisticId { get; set; }

    public DateOnly ReportDate { get; set; }

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }

    public int TotalQuantitySold { get; set; }

    public decimal TotalRevenue { get; set; }

    public string? TimeDimension { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
