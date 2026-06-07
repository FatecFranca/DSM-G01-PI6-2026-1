using Sleep.Communication.Responses.Sleep;

namespace Sleep.Application.UseCases.Sleep.Get.ById
{
    public interface IGetSleepRecordByIdUseCase
    {
        Task<ResponseSleepRecordDetail> Execute(long recordId);
    }
}
