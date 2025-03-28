using Project_PRN222.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_PRN222.Repositories.Interfaces
{
    public interface ISalesStatisticsRepository
    {
        Task<IEnumerable<SalesStatistic>> GetAll();
        Task<SalesStatistic> GetById(int id);
        Task<SalesStatistic> Create(SalesStatistic statistic);
        Task Update(SalesStatistic statistic);
        Task Delete(int id);
    }
}