using Microsoft.AspNetCore.Mvc;
using Project_PRN222.Models;
using Project_PRN222.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_PRN222.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesStatisticsController : ControllerBase
    {
        private readonly ISalesStatisticsService _salesStatisticsService;
        private readonly IOrderService _orderService;

        public SalesStatisticsController(ISalesStatisticsService salesStatisticsService, IOrderService orderService)
        {
            _salesStatisticsService = salesStatisticsService;
            _orderService = orderService;
        }

        // GET: api/SalesStatistics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesStatisticDto>>> GetAllStatistics()
        {
            try
            {
                var statistics = await _salesStatisticsService.GetAllStatistics();
                var statisticDtos = new List<SalesStatisticDto>();

                foreach (var stat in statistics)
                {
                    var dto = new SalesStatisticDto(stat);
                    dto.TopBuyer = await CalculateTopBuyer(stat.ReportDate, stat.ProductId, stat.CategoryId);
                    statisticDtos.Add(dto);
                }

                return Ok(statisticDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/SalesStatistics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesStatisticDto>> GetStatistic(int id)
        {
            try
            {
                var statistic = await _salesStatisticsService.GetStatisticById(id);
                if (statistic == null)
                {
                    return NotFound($"Sales statistic with ID {id} not found");
                }

                var dto = new SalesStatisticDto(statistic);
                dto.TopBuyer = await CalculateTopBuyer(statistic.ReportDate, statistic.ProductId, statistic.CategoryId);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/SalesStatistics
        [HttpPost]
        public async Task<ActionResult<SalesStatisticDto>> CreateStatistic([FromBody] SalesStatistic statistic)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdStatistic = await _salesStatisticsService.CreateStatistic(statistic);

                var dto = new SalesStatisticDto(createdStatistic);
                dto.TopBuyer = await CalculateTopBuyer(createdStatistic.ReportDate, createdStatistic.ProductId,
                    createdStatistic.CategoryId);

                return CreatedAtAction(nameof(GetStatistic), new { id = createdStatistic.StatisticId }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/SalesStatistics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatistic(int id, [FromBody] SalesStatisticDto statisticDto)
        {
            try
            {
                if (id != statisticDto.StatisticId)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingStatistic = await _salesStatisticsService.GetStatisticById(id);
                if (existingStatistic == null)
                {
                    return NotFound($"Sales statistic with ID {id} not found");
                }

                // Map DTO to entity
                existingStatistic.ReportDate = statisticDto.ReportDate;
                existingStatistic.ProductId = statisticDto.ProductId;
                existingStatistic.CategoryId = statisticDto.CategoryId;
                existingStatistic.TotalQuantitySold = statisticDto.TotalQuantitySold;
                existingStatistic.TotalRevenue = statisticDto.TotalRevenue;
                existingStatistic.TimeDimension = statisticDto.TimeDimension;

                await _salesStatisticsService.UpdateStatistic(existingStatistic);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/SalesStatistics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatistic(int id)
        {
            try
            {
                var statistic = await _salesStatisticsService.GetStatisticById(id);
                if (statistic == null)
                {
                    return NotFound($"Sales statistic with ID {id} not found");
                }

                await _salesStatisticsService.DeleteStatistic(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // GET: api/SalesStatistics/TopBuyers
        [HttpGet("TopBuyers")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopBuyers()
        {
            try
            {
                var orders = await _orderService.GetOrdersWithDetails();
        
                // Group orders by user and calculate total purchases
                var userPurchases = orders
                    .Where(o => o.UserId.HasValue && o.OrderDate.HasValue)
                    .GroupBy(o => o.UserId.Value)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        UserName = g.First().User?.UserName ?? $"User {g.Key}",
                        TotalPurchases = g.Count(),
                        TotalSpent = g.Sum(o => o.TotalAmount),
                        LastPurchaseDate = g.Max(o => o.OrderDate)
                    })
                    .OrderByDescending(u => u.TotalSpent)
                    .Take(10)
                    .ToList();

                return Ok(userPurchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to calculate the top buyer for a specific date/period
        // Helper method to calculate the top buyer for a specific date/period
// Helper method to calculate the top buyer for a specific date/period
        // Helper method to calculate the top buyer for a specific date/period
private async Task<string> CalculateTopBuyer(DateOnly reportDate, int? productId, int? categoryId)
{
    try
    {
        // Get all orders with details
        var orders = await _orderService.GetOrdersWithDetails();

        // Convert reportDate to DateTime for comparison
        var reportDateTime = reportDate.ToDateTime(TimeOnly.MinValue);

        // Filter orders by date - compare the date parts only
        var filteredOrders = orders.Where(o =>
            o.OrderDate.HasValue &&
            o.OrderDate.Value.Year == reportDateTime.Year &&
            o.OrderDate.Value.Month == reportDateTime.Month &&
            o.OrderDate.Value.Day == reportDateTime.Day
        ).ToList();

        if (!filteredOrders.Any())
        {
            return "No buyers on this date";
        }

        // Group orders by user and calculate total purchases
        var userPurchases = new Dictionary<int, (string Name, int Quantity, decimal Total)>();

        foreach (var order in filteredOrders)
        {
            // Skip orders without a user ID
            if (!order.UserId.HasValue) continue;

            var userId = order.UserId.Value;
            var userName = order.User?.UserName ?? $"User {userId}";

            // Calculate total quantity and amount for this order
            int orderQuantity = 0;
            decimal orderTotal = 0;

            foreach (var item in order.OrderItems)
            {
                // Apply product or category filter if specified
                if (productId.HasValue && item.ProductId != productId)
                    continue;

                if (categoryId.HasValue)
                {
                    // Skip if we can't determine the category or it doesn't match
                    if (item.Product?.CategoryId == null || item.Product.CategoryId != categoryId)
                        continue;
                }

                orderQuantity += item.Quantity;
                orderTotal += item.Price * item.Quantity;
            }

            // Only count this user if they purchased relevant items
            if (orderQuantity > 0)
            {
                if (userPurchases.ContainsKey(userId))
                {
                    var current = userPurchases[userId];
                    userPurchases[userId] = (current.Name, current.Quantity + orderQuantity,
                        current.Total + orderTotal);
                }
                else
                {
                    userPurchases[userId] = (userName, orderQuantity, orderTotal);
                }
            }
        }

        if (!userPurchases.Any())
        {
            return "No relevant purchases";
        }

        // Find the user with the highest total purchase value
        var topBuyer = userPurchases.OrderByDescending(p => p.Value.Total).First();
        return $"{topBuyer.Value.Name} (ID: {topBuyer.Key})";
    }
    catch (Exception ex)
    {
        return $"Could not determine top buyer: {ex.Message}";
    }
}

[HttpPost("GenerateReport")]
public async Task<ActionResult<SalesStatisticDto>> GenerateReport([FromBody] GenerateReportRequest request)
{
    try
    {
        // Kiểm tra request
        if (request == null)
        {
            return BadRequest("Request cannot be null");
        }

        // Log để debug
        Console.WriteLine($"Generating report for date: {request.ReportDate}, dimension: {request.TimeDimension}");
        
        // Convert string date to DateOnly with error handling
        DateOnly reportDate;
        if (string.IsNullOrEmpty(request.ReportDate) || !DateOnly.TryParse(request.ReportDate, out reportDate))
        {
            reportDate = DateOnly.FromDateTime(DateTime.Today);
            Console.WriteLine($"Using default date: {reportDate}");
        }

        // Generate the report for today
        var newStatistic = await _salesStatisticsService.GenerateDailyReport(
            reportDate, 
            request.TimeDimension ?? "Daily",
            request.ProductId,
            request.CategoryId);

        if (newStatistic == null)
        {
            return Ok(new { message = "No sales data found for the specified date. Report not generated." });
        }

        var dto = new SalesStatisticDto(newStatistic);
        
        // Thêm try-catch cho CalculateTopBuyer
        try {
            dto.TopBuyer = await CalculateTopBuyer(newStatistic.ReportDate, newStatistic.ProductId, newStatistic.CategoryId);
        }
        catch (Exception ex) {
            Console.WriteLine($"Error calculating top buyer: {ex.Message}");
            dto.TopBuyer = "Could not determine";
        }

        return Ok(dto);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GenerateReport: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

public class GenerateReportRequest
{
    public string ReportDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public string TimeDimension { get; set; } = "Daily";
    public int? ProductId { get; set; }
    public int? CategoryId { get; set; }
}
    }
}