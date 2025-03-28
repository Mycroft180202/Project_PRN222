using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using Project_PRN222.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_PRN222.Services.Implementations
{
    public class SalesStatisticsService : ISalesStatisticsService
    {
        private readonly ISalesStatisticsRepository _salesStatisticsRepository;
        private readonly ProjectPrn222Context _context;
        private readonly IOrderService _orderService;

        public SalesStatisticsService(ISalesStatisticsRepository salesStatisticsRepository, ProjectPrn222Context context, IOrderService _orderService)
        {
            _salesStatisticsRepository = salesStatisticsRepository;
            _context = context;
            _orderService = _orderService;
        }
       

        

        public async Task<IEnumerable<SalesStatistic>> GetAllStatistics()
        {
            return await _salesStatisticsRepository.GetAll();
        }

        public async Task<SalesStatistic> GetStatisticById(int id)
        {
            return await _salesStatisticsRepository.GetById(id);
        }

        public async Task<SalesStatistic> CreateStatistic(SalesStatistic statistic)
        {
            if (statistic == null)
            {
                throw new ArgumentNullException(nameof(statistic));
            }

            // Set default values if not provided
            if (statistic.ReportDate == default)
            {
                statistic.ReportDate = DateOnly.FromDateTime(DateTime.Today);
            }

            return await _salesStatisticsRepository.Create(statistic);
        }

        public async Task UpdateStatistic(SalesStatistic statistic)
        {
            if (statistic == null)
            {
                throw new ArgumentNullException(nameof(statistic));
            }

            await _salesStatisticsRepository.Update(statistic);
        }

        public async Task DeleteStatistic(int id)
        {
            await _salesStatisticsRepository.Delete(id);
        }
        public async Task<SalesStatistic> GenerateDailyReport(DateOnly reportDate, string timeDimension, int? productId = null, int? categoryId = null)
{
    try
    {
        // Kiểm tra null và gán giá trị mặc định
        timeDimension = timeDimension ?? "Daily";
        
        Console.WriteLine($"Service: Generating report for {reportDate}, dimension: {timeDimension}");
        
        // Check if a report already exists for this date, product, and category
        var existingReport = await _context.SalesStatistics
            .FirstOrDefaultAsync(s => 
                s.ReportDate == reportDate && 
                s.ProductId == productId && 
                s.CategoryId == categoryId &&
                s.TimeDimension == timeDimension);

        if (existingReport != null)
        {
            // Update existing report with new data
            var updatedStats = await CalculateSalesStatistics(reportDate, timeDimension, productId, categoryId);
            existingReport.TotalQuantitySold = updatedStats.TotalQuantitySold;
            existingReport.TotalRevenue = updatedStats.TotalRevenue;
            
            await _context.SaveChangesAsync();
            return existingReport;
        }
        else
        {
            // Create a new report
            var newStatistic = await CalculateSalesStatistics(reportDate, timeDimension, productId, categoryId);
            
            // Luôn trả về một đối tượng, ngay cả khi không có dữ liệu bán hàng
            if (newStatistic.TotalQuantitySold == 0)
            {
                newStatistic.TotalQuantitySold = 0;
                newStatistic.TotalRevenue = 0;
            }
            
            _context.SalesStatistics.Add(newStatistic);
            await _context.SaveChangesAsync();
            return newStatistic;
        }
    }
    catch (Exception ex)
    {
        // Log the exception
        Console.WriteLine($"Error generating daily report: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}

        private async Task<SalesStatistic> CalculateSalesStatistics(DateOnly reportDate, string timeDimension, int? productId = null, int? categoryId = null)
{
    try {
        // Get all orders with details
        var orders = await _orderService.GetOrdersWithDetails();
        
        // Kiểm tra null
        if (orders == null)
        {
            Console.WriteLine("Warning: Orders is null");
            orders = new List<Order>();
        }
        
        // Convert reportDate to DateTime for comparison
        var reportDateTime = reportDate.ToDateTime(TimeOnly.MinValue);
        
        // Filter orders by date based on the time dimension
        var filteredOrders = orders.Where(o => o.OrderDate.HasValue).ToList();
        
        // Kiểm tra null và gán giá trị mặc định
        timeDimension = timeDimension ?? "Daily";
        
        switch (timeDimension.ToLower())
        {
            case "daily":
                filteredOrders = filteredOrders.Where(o => 
                    o.OrderDate.Value.Year == reportDateTime.Year &&
                    o.OrderDate.Value.Month == reportDateTime.Month &&
                    o.OrderDate.Value.Day == reportDateTime.Day).ToList();
                break;
                
            case "weekly":
                // Get the start of the week (assuming Monday is the first day)
                var startOfWeek = reportDateTime.AddDays(-(int)reportDateTime.DayOfWeek + 1);
                if (reportDateTime.DayOfWeek == DayOfWeek.Sunday) startOfWeek = reportDateTime.AddDays(-6);
                var endOfWeek = startOfWeek.AddDays(6);
                
                filteredOrders = filteredOrders.Where(o => 
                    o.OrderDate.Value >= startOfWeek &&
                    o.OrderDate.Value <= endOfWeek).ToList();
                break;
                
            case "monthly":
                filteredOrders = filteredOrders.Where(o => 
                    o.OrderDate.Value.Year == reportDateTime.Year &&
                    o.OrderDate.Value.Month == reportDateTime.Month).ToList();
                break;
                
            case "yearly":
                filteredOrders = filteredOrders.Where(o => 
                    o.OrderDate.Value.Year == reportDateTime.Year).ToList();
                break;
                
            default:
                // Default to daily
                filteredOrders = filteredOrders.Where(o => 
                    o.OrderDate.Value.Year == reportDateTime.Year &&
                    o.OrderDate.Value.Month == reportDateTime.Month &&
                    o.OrderDate.Value.Day == reportDateTime.Day).ToList();
                break;
        }
        
        // Calculate total quantity sold and revenue
        int totalQuantitySold = 0;
        decimal totalRevenue = 0;
        
        foreach (var order in filteredOrders)
        {
            // Kiểm tra null cho OrderItems
            if (order.OrderItems == null)
            {
                Console.WriteLine($"Warning: OrderItems is null for order {order.OrderId}");
                continue;
            }
            
            foreach (var item in order.OrderItems)
            {
                // Kiểm tra null cho item
                if (item == null)
                {
                    Console.WriteLine("Warning: OrderItem is null");
                    continue;
                }
                
                // Apply product or category filter if specified
                if (productId.HasValue && item.ProductId != productId)
                    continue;
                    
                if (categoryId.HasValue)
                {
                    // Đây có thể là dòng 123 gây ra lỗi - item.Product có thể là null
                    // Kiểm tra null cho Product
                    if (item.Product == null)
                    {
                        Console.WriteLine($"Warning: Product is null for item in order {order.OrderId}, item ID: {item.OrderItemId}");
                        continue;
                    }
                    
                    // Skip if we can't determine the category or it doesn't match
                    if (item.Product.CategoryId == null || item.Product.CategoryId != categoryId)
                        continue;
                }
                
                totalQuantitySold += item.Quantity;
                totalRevenue += item.Price * item.Quantity;
            }
        }
        
        // Create and return the statistic
        return new SalesStatistic
        {
            ReportDate = reportDate,
            ProductId = productId,
            CategoryId = categoryId,
            TotalQuantitySold = totalQuantitySold,
            TotalRevenue = totalRevenue,
            TimeDimension = timeDimension
        };
    }
    catch (Exception ex) {
        Console.WriteLine($"Error in CalculateSalesStatistics: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        // Return empty statistics instead of throwing
        return new SalesStatistic
        {
            ReportDate = reportDate,
            ProductId = productId,
            CategoryId = categoryId,
            TotalQuantitySold = 0,
            TotalRevenue = 0,
            TimeDimension = timeDimension ?? "Daily"
        };
    }
}
        
    }
}