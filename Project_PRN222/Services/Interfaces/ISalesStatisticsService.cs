using Project_PRN222.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_PRN222.Services.Interfaces
{
    public interface ISalesStatisticsService
    {
        Task<IEnumerable<SalesStatistic>> GetAllStatistics();
        Task<SalesStatistic> GetStatisticById(int id);
        Task<SalesStatistic> CreateStatistic(SalesStatistic statistic);
        Task UpdateStatistic(SalesStatistic statistic);
        Task DeleteStatistic(int id);
        
    }
}