using Sleep.Domain.Dtos;

namespace Sleep.Domain.Repositories.SleepAnalysis
{
    public interface ISleepAnalysisRepositoryReadOnly
    {
        Task<Entities.SleepAnalysis?> GetBySleepRecordId(long sleepRecord);
        Task<IReadOnlyList<Entities.SleepAnalysis>> ListByUserIdAsync(long userId);
        Task<List<SleepAnalysisScoreDto>> GetAnalysisById(long sleepRecordId);
        Task<List<SleepAnalysisScoreDto>> GetAnalysisByIds(List<long> sleepRecordIds);
    }
}
