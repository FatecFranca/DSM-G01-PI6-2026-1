using Sleep.Communication.Responses.Sleep;

namespace Sleep.Application.UseCases.Sleep.Get.Analysis
{
    public interface IGetSleepAnalysisUseCase
    {
        Task<ResponseSleepAnalysis> Execute(long recordId);
    }
}
