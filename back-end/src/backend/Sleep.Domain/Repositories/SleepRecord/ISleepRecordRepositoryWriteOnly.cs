using Sleep.Domain.Entities;

namespace Sleep.Domain.Repositories.SleepRecord
{
    public interface ISleepRecordRepositoryWriteOnly
    {
        public Task<long> Add(Entities.SleepRecord record);
    }
}
