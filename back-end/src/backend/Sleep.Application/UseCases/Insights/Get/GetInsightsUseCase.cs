using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Responses.Insights;
using Sleep.Domain.Dtos;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Domain.Utils.Page;

namespace Sleep.Application.UseCases.Insights.Get
{
    public class GetInsightsUseCase : IGetInsightsUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepRecordRepositoryReadOnly _sleepRecordRepository;
        private readonly ISleepAnalysisRepositoryReadOnly _sleepAnalysisRepository;

        public GetInsightsUseCase(
            ILoggedUser loggedUser,
            ISleepRecordRepositoryReadOnly sleepRecordRepository,
            ISleepAnalysisRepositoryReadOnly sleepAnalysisRepository)
        {
            _loggedUser = loggedUser;
            _sleepRecordRepository = sleepRecordRepository;
            _sleepAnalysisRepository = sleepAnalysisRepository;
        }

        public async Task<ResponseInsights> Execute()
        {
            var user = await _loggedUser.User();
            var filter = new SleepHistoryFilterDto
            {
                SleepStart = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)),
                SleepEnd = DateOnly.FromDateTime(DateTime.Now)
            };

            var history = await _sleepRecordRepository.ListSleepRecordsAsync(
                user.Id,
                new PageParameters { PageNumber = 1, PageSize = 100 },
                filter);

            var records = history.Items;
            var analyses = await _sleepAnalysisRepository.ListByUserIdAsync(user.Id);

            if (records.Count == 0)
            {
                return new ResponseInsights
                {
                    AverageSleep = 0,
                    AverageScore = null,
                    Patterns = ["Ainda nao ha registros suficientes para gerar padroes."],
                    Recommendations = ["Preencha mais registros diarios para liberar insights personalizados."]
                };
            }

            var durations = records
                .Select(record => (double)record.DurationHours)
                .Where(duration => duration > 0)
                .ToList();

            var averageSleep = durations.Count > 0
                ? durations.Average()
                : 0;

            var scores = analyses
                .Select(analysis => (double)analysis.Score)
                .Where(score => score > 0)
                .ToList();

            double? averageScore = scores.Count > 0 ? scores.Average() : null;

            var patterns = new List<string>
            {
                $"Foram analisados {records.Count} registros nos ultimos 30 dias.",
                $"Media de sono aproximada: {averageSleep:F1} horas por noite."
            };

            if (averageScore.HasValue)
            {
                patterns.Add($"Score medio de qualidade: {averageScore.Value:F0}%.");
            }

            var recommendations = new List<string>
            {
                "Mantenha o registro diario para aumentar a precisao dos insights."
            };

            if (averageSleep < 7)
            {
                recommendations.Add("Tente dormir pelo menos 7 horas por noite para melhorar a recuperacao.");
            }
            else
            {
                recommendations.Add("Continue mantendo uma rotina consistente de sono.");
            }

            if (records.Any(record => record.StressLevel >= 7))
            {
                recommendations.Add("Seu nivel de estresse recente esta elevado; considere tecnicas de relaxamento antes de dormir.");
            }

            return new ResponseInsights
            {
                AverageSleep = averageSleep,
                AverageScore = averageScore,
                Patterns = patterns,
                Recommendations = recommendations
            };
        }
    }
}
