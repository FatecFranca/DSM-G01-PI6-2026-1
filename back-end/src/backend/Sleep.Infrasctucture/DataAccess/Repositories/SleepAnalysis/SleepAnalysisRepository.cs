using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.DataAccess.Repositories.SleepAnalysis
{
    public class SleepAnalysisRepository : ISleepAnalysisRepositoryWriteOnly, ISleepAnalysisRepositoryReadOnly
    {
        private readonly SleepDbContext _dbContext;

        public SleepAnalysisRepository(SleepDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Domain.Entities.SleepAnalysis record) => await _dbContext.SleepAnalysis.AddAsync(record);

        public async Task<Domain.Entities.SleepAnalysis?> GetBySleepRecordId(long sleepRecord)
        {
            return await _dbContext.SleepAnalysis
                .AsNoTracking()
                .Where(r => r.SleepRecordId == sleepRecord)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Domain.Entities.SleepAnalysis>> ListByUserIdAsync(long userId)
        {
            return await _dbContext.SleepAnalysis
                .AsNoTracking()
                .Where(analysis => 
                    _dbContext.SleepRecord.Any(record =>
                    record.Id == analysis.SleepRecordId && record.UserId == userId))
                .ToListAsync();
        }
    }
}
