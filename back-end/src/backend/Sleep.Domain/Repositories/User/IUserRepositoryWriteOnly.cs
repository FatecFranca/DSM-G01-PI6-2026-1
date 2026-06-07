namespace Sleep.Domain.Repositories.User
{
    public interface IUserRepositoryWriteOnly
    {
        public Task Add(Entities.User user);
        public Task Update(Entities.User user);
    }
}
