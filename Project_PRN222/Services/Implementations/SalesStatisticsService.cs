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

        public SalesStatisticsService(ISalesStatisticsRepository salesStatisticsRepository)
        {
            _salesStatisticsRepository = salesStatisticsRepository;
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
        
    }
}