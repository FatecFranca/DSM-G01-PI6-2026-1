namespace Sleep.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public Task Commit();
    }
}
