using Sleep.Domain.Repositories;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SleepDbContext _dbContext;

        public UnitOfWork(SleepDbContext dbContext) => _dbContext = dbContext;

        public async Task Commit() => await _dbContext.SaveChangesAsync(); 
    }
}
