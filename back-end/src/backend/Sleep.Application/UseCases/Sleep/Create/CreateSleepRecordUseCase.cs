using AutoMapper;
using Sleep.Application.Services.LoggedUser;
using Sleep.Application.UseCases.Sleep.Anlysis;
using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Utils;
using Sleep.Domain.Entities;
using Sleep.Domain.Messaging;
using Sleep.Domain.Repositories;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Exceptions.ExceptionsBase;
using System.Runtime.CompilerServices;

namespace Sleep.Application.UseCases.Sleep.Create
{
    public class CreateSleepRecordUseCase : ICreateSleepRecordUseCase
    {
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepRecordRepositoryWriteOnly _writeRepository;
        private readonly ISleepRecordRepositoryReadOnly _readRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDoSleepAnalysisUseCase _analysisUseCase;

        public CreateSleepRecordUseCase(IMapper mapper, ILoggedUser loggedUser, ISleepRecordRepositoryWriteOnly writeRepository, ISleepRecordRepositoryReadOnly readRepository, IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, IDoSleepAnalysisUseCase analysisUseCase)
        {
            _mapper = mapper;
            _loggedUser = loggedUser;
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _unitOfWork = unitOfWork;
            _analysisUseCase = analysisUseCase;
        }

        public async Task Execute(RequestCreateSleepRecord request)
        {
            await Validate(request);
            var user = await _loggedUser.User();
            var sleepRecord = _mapper.Map<SleepRecord>(request);

            sleepRecord.DurationHours = DateTimeHelper.DifferenceInHourBetweenDates(request.SleepStart, request.SleepEnd);
            sleepRecord.Age = DateTimeHelper.DifferenceInYearBetweenDates(DateTime.Now,user.BirthDate);
            sleepRecord.UserId = user.Id;

            //Validação UK
            var recordDate = request.SleepStart.Hour < 12 ? request.SleepStart.AddDays(-1) : request.SleepStart;
            var recordDateOnly = DateOnly.FromDateTime(recordDate);

            var existRecordDate = await _readRepository.ExistWithRecordDateAndUserId(recordDateOnly, user.Id);
            if (existRecordDate)
                throw new InvalidRecordDateException();

            sleepRecord.RecordDate = recordDateOnly;

            var recordId = await _writeRepository.Add(sleepRecord);
            await _unitOfWork.Commit();
            await _analysisUseCase.Execute(recordId);
        }

        public async Task Validate(RequestCreateSleepRecord request)
        {
            var validator = new CreateSleepRecordValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
