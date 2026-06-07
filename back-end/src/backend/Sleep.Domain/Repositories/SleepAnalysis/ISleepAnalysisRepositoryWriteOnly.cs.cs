using Sleep.Domain.Entities;

namespace Sleep.Domain.Repositories.SleepAnalysis
{
    public interface ISleepAnalysisRepositoryWriteOnly
    {
        Task Add(Entities.SleepAnalysis record);
    }
}
