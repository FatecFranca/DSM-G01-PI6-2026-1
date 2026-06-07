using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Application.UseCases.Sleep.Get.ById
{
    public class GetSleepRecordByIdUseCase : IGetSleepRecordByIdUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepRecordRepositoryReadOnly _sleepRecordRepository;
        private readonly ISleepAnalysisRepositoryReadOnly _sleepAnalysisRepository;

        public GetSleepRecordByIdUseCase(
            ILoggedUser loggedUser,
            ISleepRecordRepositoryReadOnly sleepRecordRepository,
            ISleepAnalysisRepositoryReadOnly sleepAnalysisRepository)
        {
            _loggedUser = loggedUser;
            _sleepRecordRepository = sleepRecordRepository;
            _sleepAnalysisRepository = sleepAnalysisRepository;
        }

        public async Task<ResponseSleepRecordDetail> Execute(long recordId)
        {
            var user = await _loggedUser.User();
            var sleepRecord = await _sleepRecordRepository.GetById(recordId, user.Id)
                ?? throw new NotFoundException();

            var analysis = await _sleepAnalysisRepository.GetBySleepRecordId(recordId);

            return new ResponseSleepRecordDetail
            {
                SleepRecordId = sleepRecord.Id,
                RecordDate = sleepRecord.RecordDate,
                DurationInHours = sleepRecord.DurationHours,
                SleepQuality = sleepRecord.QualityOfSleep,
                StressLevel = sleepRecord.StressLevel,
                PhysicalActivityMinutes = sleepRecord.PhysicalActivityMinutes,
                DailySteps = sleepRecord.DailySteps,
                BloodPressure = sleepRecord.BloodPressure,
                ScreenBeforeSleep = sleepRecord.ScreenBeforeSleep,
                Caffeine = sleepRecord.Caffeine,
                Alcohol = sleepRecord.Alcohol,
                Notes = sleepRecord.Notes,
                HeartRate = sleepRecord.HeartRate,
                MentalFatigue = sleepRecord.MentalFigure,
                SleepScore = analysis?.Score
            };
        }
    }
}
