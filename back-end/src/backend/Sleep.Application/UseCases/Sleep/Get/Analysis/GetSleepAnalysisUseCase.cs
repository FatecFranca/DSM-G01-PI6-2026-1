using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Application.UseCases.Sleep.Get.Analysis
{
    public class GetSleepAnalysisUseCase : IGetSleepAnalysisUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepRecordRepositoryReadOnly _sleepRecordRepository;
        private readonly ISleepAnalysisRepositoryReadOnly _sleepAnalysisRepository;

        public GetSleepAnalysisUseCase(
            ILoggedUser loggedUser,
            ISleepRecordRepositoryReadOnly sleepRecordRepository,
            ISleepAnalysisRepositoryReadOnly sleepAnalysisRepository)
        {
            _loggedUser = loggedUser;
            _sleepRecordRepository = sleepRecordRepository;
            _sleepAnalysisRepository = sleepAnalysisRepository;
        }

        public async Task<ResponseSleepAnalysis> Execute(long recordId)
        {
            var user = await _loggedUser.User();
            var sleepRecord = await _sleepRecordRepository.GetById(recordId, user.Id)
                ?? throw new NotFoundException();

            var analysis = await _sleepAnalysisRepository.GetBySleepRecordId(sleepRecord.Id)
                ?? throw new NotFoundException();

            return new ResponseSleepAnalysis
            {
                Score = analysis.Score,
                Problem = analysis.Problem
            };
        }
    }
}
