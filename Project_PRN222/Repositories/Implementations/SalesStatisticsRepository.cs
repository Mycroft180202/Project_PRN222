using Microsoft.EntityFrameworkCore;
using Project_PRN222.Models;
using Project_PRN222.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_PRN222.Repositories.Implementations
{
    public class SalesStatisticsRepository : ISalesStatisticsRepository
    {
        private readonly ProjectPrn222Context _context;

        public SalesStatisticsRepository(ProjectPrn222Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesStatistic>> GetAll()
        {
            return await _context.SalesStatistics
                .Include(s => s.Product)
                .Include(s => s.Category)
                .ToListAsync();
        }

        public async Task<SalesStatistic> GetById(int id)
        {
            return await _context.SalesStatistics
                .Include(s => s.Product)
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.StatisticId == id);
        }

        public async Task<SalesStatistic> Create(SalesStatistic statistic)
        {
            _context.SalesStatistics.Add(statistic);
            await _context.SaveChangesAsync();
            return statistic;
        }

        public async Task Update(SalesStatistic statistic)
        {
            _context.Entry(statistic).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var statistic = await _context.SalesStatistics.FindAsync(id);
            if (statistic != null)
            {
                _context.SalesStatistics.Remove(statistic);
                await _context.SaveChangesAsync();
            }
        }
    }
}