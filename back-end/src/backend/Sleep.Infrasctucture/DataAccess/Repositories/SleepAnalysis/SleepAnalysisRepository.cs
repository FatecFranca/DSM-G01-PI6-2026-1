using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Dtos;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.DataAccess.Repositories.SleepAnalysis
{
    public class SleepAnalysisRepository : ISleepAnalysisRepositoryWriteOnly, ISleepAnalysisRepositoryReadOnly
    {
        private readonly SleepDbContext _dbContext;

        public SleepAnalysisRepository(SleepDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Domain.Entities.SleepAnalysis record) => await _dbContext.SleepAnalysis.AddAsync(record);

        public Task<List<SleepAnalysisScoreDto>> GetAnalysisById(long sleepRecordId)
        {
            return _dbContext
                .SleepAnalysis
                .Where(u => u.SleepRecordId == sleepRecordId)
                .Select(rec => new SleepAnalysisScoreDto
                {
                    SleepId = rec.SleepRecordId,
                    SleepScore = rec.Score
                })
                .ToListAsync();
        }

        public Task<List<SleepAnalysisScoreDto>> GetAnalysisByIds(List<long> sleepRecordIds)
        {
            return _dbContext
                .SleepAnalysis
                .Where(u => sleepRecordIds.Contains(u.SleepRecordId))
                .Select(rec => new SleepAnalysisScoreDto
                {
                    SleepId = rec.SleepRecordId,
                    SleepScore = rec.Score
                })
                .ToListAsync();
        }

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
