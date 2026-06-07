using Sleep.Communication.Requests.Sleep;

namespace Sleep.Application.UseCases.Sleep.Create
{
    public interface ICreateSleepRecordUseCase
    {
        public Task Execute(RequestCreateSleepRecord request);
    }
}
