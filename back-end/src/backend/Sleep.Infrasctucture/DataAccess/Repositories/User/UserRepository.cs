using Microsoft.EntityFrameworkCore;
using Sleep.Domain.Entities;
using Sleep.Domain.Enum;
using Sleep.Domain.Repositories.User;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.DataAccess.Repositories.User
{
    public class UserRepository : IUserRepositoryReadOnly, IUserRepositoryWriteOnly
    {
        private readonly SleepDbContext _dbContext;

        public UserRepository(SleepDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Domain.Entities.User user) => await _dbContext.AddAsync(user);

        public Task Update(Domain.Entities.User user)
        {
            _dbContext.Users.Update(user);
            return Task.CompletedTask;
        }


        public async Task<bool> ExistUserWithEmail(string email)
        {
            return await _dbContext
                .Users
                .AsNoTracking()
                .AnyAsync(user => user.Email.Equals(email));
        }

        public async Task<Domain.Entities.User?> ExistUserWithEmailAndPassword(string email, string password)
        {
            return await _dbContext
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Equals(email) && u.Password.Equals(password));
        }

        public async Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier)
        {
            return await _dbContext
                .Users
                .AnyAsync(user => user.UserIdentifier.Equals(userIdentifier) && user.Status == UserStatus.Active);
        }
    }
}
