using AutoMapper;
using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Dtos;
using Sleep.Domain.Repositories.SleepRecord;
using Sleep.Domain.Utils.Page;
using Sleep.Exceptions.ExceptionsBase;
using System.Xml;

namespace Sleep.Application.UseCases.Sleep.Get.SleepHistory
{
    public class GetSleepHistoryUseCase : IGetSleepHistoryUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly ISleepRecordRepositoryReadOnly _sleepRepoReadOnly;
        private readonly IMapper _mapper;

        public GetSleepHistoryUseCase(ILoggedUser loggedUser, ISleepRecordRepositoryReadOnly sleepRepoReadOnly, IMapper mapper)
        {
            _loggedUser = loggedUser;
            _sleepRepoReadOnly = sleepRepoReadOnly;
            _mapper = mapper;
        }

        public async Task<PagedList<ShortSleepRecord>> Execute(PageParameters pageParameters, RequestSleepHistoryFilter requestFilter)
        {
            var user = await _loggedUser.User();
            Validate(requestFilter);
            var filterDto = new SleepHistoryFilterDto
            {
                SleepStart = requestFilter.SleepStart ?? DateOnly.FromDateTime(DateTime.Now.AddDays(-7)),
                SleepEnd = requestFilter.SleepEnd ?? DateOnly.FromDateTime(DateTime.Now)
            };

            var history = await _sleepRepoReadOnly.ListSleepRecordsAsync(user.Id, pageParameters, filterDto);
            var response = _mapper.Map<PagedList<ShortSleepRecord>>(history);
            return response;
        }

        public static void Validate(RequestSleepHistoryFilter filterRequest)
        {
            var validator = new GetSleepHistoryValidator();
            var result = validator.Validate(filterRequest);


            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
