namespace Sleep.Application.UseCases.Sleep.Anlysis
{
    public interface IDoSleepAnalysisUseCase
    {
        Task Execute(long sleepRecordId);
    }
}
