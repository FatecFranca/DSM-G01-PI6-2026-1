using Microsoft.Extensions.Logging;
using Sleep.Communication.Events;
using Sleep.Domain.Entities;
using Sleep.Domain.Messaging;
using Sleep.Domain.Repositories;
using Sleep.Domain.Repositories.SleepAnalysis;

namespace Sleep.Application.UseCases.Sleep.Anlysis.Messaging
{
    public class SleepAnalysisResponseHandler : IMessageHandler<SleepAnalysisResponseMessage>
    {
        private readonly ISleepAnalysisRepositoryWriteOnly _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SleepAnalysisResponseHandler(
            ISleepAnalysisRepositoryWriteOnly repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(SleepAnalysisResponseMessage message, CancellationToken ct)
        {
            var sleepAnalysis = new SleepAnalysis 
            {
                SleepRecordId = message.SleepRecordId,
                Problem = message.Problem,
                Score = message.Score,
            };
            await _repository.Add(sleepAnalysis);
            await _unitOfWork.Commit();
        }
    }
}
