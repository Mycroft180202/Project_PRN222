using Project_PRN222.Models;

public class SalesStatisticDto
{
    // Original properties from SalesStatistic
    public int StatisticId { get; set; }
    public DateOnly ReportDate { get; set; }
    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
    public string? TimeDimension { get; set; }
    
    // Additional property for frontend display only
    public string? TopBuyer { get; set; }
    
    // Constructor to convert from SalesStatistic
    public SalesStatisticDto(SalesStatistic statistic)
    {
        StatisticId = statistic.StatisticId;
        ReportDate = statistic.ReportDate;
        ProductId = statistic.ProductId;
        CategoryId = statistic.CategoryId;
        TotalQuantitySold = statistic.TotalQuantitySold;
        TotalRevenue = statistic.TotalRevenue;
        TimeDimension = statistic.TimeDimension;
        TopBuyer = null; // Will be populated by the service
    }
}