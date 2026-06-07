using Sleep.Domain.Entities;

namespace Sleep.Application.Services.LoggedUser
{
    public interface ILoggedUser
    {
        public Task<User> User();
    }
}
