using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Dtos;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Domain.Utils.Page;
using Sleep.Infrasctructure.Utils.Page;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.DataAccess.Repositories.SleepRecord
{
    public class SleepRecordRepository : ISleepRecordRepositoryReadOnly, ISleepRecordRepositoryWriteOnly
    {
        private readonly SleepDbContext _dbContext;

        public SleepRecordRepository(SleepDbContext dbContext) => _dbContext = dbContext;

        public async Task<long> Add(Domain.Entities.SleepRecord record)
        {
            await _dbContext.SleepRecord.AddAsync(record);
            _dbContext.SaveChanges();
            return record.Id;
        }

        public async Task<bool> ExistWithRecordDateAndUserId(DateOnly recordDate, long userId)
        {
            return await _dbContext
                .SleepRecord
                .Where(s => s.UserId == userId && s.RecordDate == recordDate)
                .AnyAsync();
        }

        public async Task<Domain.Entities.SleepRecord?> GetById(long recordId, long userId)
        {
            return await _dbContext
                .SleepRecord
                .AsNoTracking()
                .Where(r => r.Id == recordId)
                .Where(r => r.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<Domain.Entities.SleepRecord>> ListSleepRecordsAsync(long userId, PageParameters pageParameters, SleepHistoryFilterDto filterDto)
        {
            var items = _dbContext
                .SleepRecord
                .Where(t => t.UserId == userId)
                .Where(t => t.RecordDate >= filterDto.SleepStart)
                .Where(t => t.RecordDate <= filterDto.SleepEnd)
                .OrderByDescending(t => t.RecordDate);

            return await PagedListExtensions.ToPagedListAsync(items, pageParameters.PageNumber, pageParameters.PageSize);   
        }
    }
}
