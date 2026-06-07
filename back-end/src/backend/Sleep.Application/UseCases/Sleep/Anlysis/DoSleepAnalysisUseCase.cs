using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Events;
using Sleep.Domain.Messaging;
using Sleep.Domain.Repositories.SleepAnalysis;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Exceptions;
using Sleep.Exceptions.ExceptionsBase;

namespace Sleep.Application.UseCases.Sleep.Anlysis
{
    public class DoSleepAnalysisUseCase : IDoSleepAnalysisUseCase
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ISleepRecordRepositoryReadOnly _readSleepRecordRepo;
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepAnalysisRepositoryReadOnly _sleepAnalysisReadRepo;

        public DoSleepAnalysisUseCase(IMessagePublisher messagePublisher, ISleepRecordRepositoryReadOnly readSleepRecordRepo, ILoggedUser loggedUser, ISleepAnalysisRepositoryReadOnly sleepAnalysisReadRepo)
        {
            _messagePublisher = messagePublisher;
            _readSleepRecordRepo = readSleepRecordRepo;
            _loggedUser = loggedUser;
            _sleepAnalysisReadRepo = sleepAnalysisReadRepo;
        }

        public async Task Execute(long sleepRecordId)
        {
            var user = await _loggedUser.User();
            var sleepRecord = await _readSleepRecordRepo.GetById(sleepRecordId, user.Id) ?? throw new NotFoundException();

            var sleepAnalysis = await _sleepAnalysisReadRepo.GetBySleepRecordId(sleepRecordId);
            if (sleepAnalysis != null)
                throw new BadRequestException(ResourceMessageException.ALREADY_EXISTS_ANALYSIS);

            var message = SleepAnalysisRequestMessage.FromSleepRecord(sleepRecord, user.Gender.ToString(), user.Occupation, sleepRecordId);
            await _messagePublisher.PublishAsync(message);
        }
    }
}
