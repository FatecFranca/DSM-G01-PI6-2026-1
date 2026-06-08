using AutoMapper;
using Sleep.Application.Services.LoggedUser;
using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Dtos;
using Sleep.Domain.Repositories.SleepAnalysis;
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
        private readonly ISleepAnalysisRepositoryReadOnly _sleepAnalysisReadOnly;

        public GetSleepHistoryUseCase(ILoggedUser loggedUser, ISleepRecordRepositoryReadOnly sleepRepoReadOnly, IMapper mapper, ISleepAnalysisRepositoryReadOnly sleepAnalysisReadOnly)
        {
            _loggedUser = loggedUser;
            _sleepRepoReadOnly = sleepRepoReadOnly;
            _mapper = mapper;
            _sleepAnalysisReadOnly = sleepAnalysisReadOnly;
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

            var sleepRecordIds = history.Items.Select(h => h.Id).ToList();

            var analysisScores = await _sleepAnalysisReadOnly.GetAnalysisByIds(sleepRecordIds);

            var scoresDictionary = analysisScores.ToDictionary(score => score.SleepId, score => score.SleepScore);

            var response = _mapper.Map<PagedList<ShortSleepRecord>>(history);

            foreach (var item in response.Items)
            {
                if (scoresDictionary.TryGetValue(item.SleepRecordId, out var score))
                {
                    item.SleepScore = score;
                }
            }

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
