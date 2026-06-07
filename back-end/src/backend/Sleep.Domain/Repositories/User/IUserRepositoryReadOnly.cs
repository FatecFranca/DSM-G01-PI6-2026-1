namespace Sleep.Domain.Repositories.User
{
    public interface IUserRepositoryReadOnly
    {
        public Task<bool> ExistUserWithEmail(string email);
        public Task<Entities.User?> ExistUserWithEmailAndPassword(string email, string password);
        public Task<bool> ExistActiveUserWithIdentifier(Guid userIdentifier);
    }
}
