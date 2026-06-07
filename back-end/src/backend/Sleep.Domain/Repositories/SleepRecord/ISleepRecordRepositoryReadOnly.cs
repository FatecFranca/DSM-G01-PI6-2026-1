using Sleep.Domain.Dtos;
using Sleep.Domain.Utils.Page;
using System.Diagnostics.Metrics;

namespace Sleep.Domain.Repositories.SleepRecord
{
    public interface ISleepRecordRepositoryReadOnly
    {
        public Task<bool> ExistWithRecordDateAndUserId(DateOnly recordDate, long userId);
        public Task<PagedList<Entities.SleepRecord>> ListSleepRecordsAsync(long userId, PageParameters pageParameters,SleepHistoryFilterDto filterDto);
        public Task<Entities.SleepRecord?> GetById(long recordId, long userId);
    }
}
