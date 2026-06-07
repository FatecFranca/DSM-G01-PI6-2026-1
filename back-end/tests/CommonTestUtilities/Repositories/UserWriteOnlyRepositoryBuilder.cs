using Moq;
using Sleep.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserWriteOnlyRepositoryBuilder
    {
        public static IUserRepositoryWriteOnly Build()
        {
            var mock = new Mock<IUserRepositoryWriteOnly>();
            return mock.Object;
        }
    }
}
