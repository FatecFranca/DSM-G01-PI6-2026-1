using Moq;
using Sleep.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserReadOnlyRepositoryBuilder
    {
        private readonly Mock<IUserRepositoryReadOnly> _repository;

        public UserReadOnlyRepositoryBuilder() => _repository = new Mock<IUserRepositoryReadOnly>();

        public IUserRepositoryReadOnly Build() => _repository.Object;

        public void ExistActiveUserWithEmail(string email)
        {
            _repository.Setup(repository => repository.ExistUserWithEmail(email)).ReturnsAsync(true);
        }
    }
}
