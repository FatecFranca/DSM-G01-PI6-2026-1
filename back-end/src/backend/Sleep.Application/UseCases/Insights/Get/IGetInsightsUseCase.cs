using Sleep.Communication.Responses.Insights;

namespace Sleep.Application.UseCases.Insights.Get
{
    public interface IGetInsightsUseCase
    {
        Task<ResponseInsights> Execute();
    }
}
