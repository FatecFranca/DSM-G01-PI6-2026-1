using Sleep.Domain.Entities;

namespace Sleep.Domain.Repositories.SleepAnalysis
{
    public interface ISleepAnalysisRepositoryReadOnly
    {
        Task<Entities.SleepAnalysis?> GetBySleepRecordId(long sleepRecord);
        Task<IReadOnlyList<Entities.SleepAnalysis>> ListByUserIdAsync(long userId);
    }
}
