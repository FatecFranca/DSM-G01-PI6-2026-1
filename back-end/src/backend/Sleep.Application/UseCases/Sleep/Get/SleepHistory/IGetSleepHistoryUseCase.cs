using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Utils.Page;

namespace Sleep.Application.UseCases.Sleep.Get.SleepHistory
{
    public interface IGetSleepHistoryUseCase
    {
        Task<PagedList<ShortSleepRecord>> Execute(PageParameters pageParameters, RequestSleepHistoryFilter requestFilter);
    }
}
