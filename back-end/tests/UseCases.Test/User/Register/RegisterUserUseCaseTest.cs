using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Services;
using FluentAssertions;
using Sleep.Application.UseCases.User.Register;
using Sleep.Exceptions;
using Sleep.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase();

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
        }

        [Fact]
        public async Task Error_Email_Already_Registered()
        {
            var request = RequestRegisterUserJsonBuilder.Build();
            var useCase = CreateUseCase(request.Email);

            Func<Task> act = async () => await useCase.Execute(request);
            (await act.Should().ThrowAsync<ErrorOnValidationException>())
                .Where(e => e.GetErrorMessages().Count == 1 &&
                        e.GetErrorMessages().Contains(ResourceMessageException.EMAIL_ALREADY_REGISTERED));
        }


        private static RegisterUserUseCase CreateUseCase(string? email = null, string? cpf = null)
        {
            var mapper = MappingBuilder.Build();
            var passwordEncrypter = PasswordEncrypterBuilder.Build();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var writeRepository = UserWriteOnlyRepositoryBuilder.Build();
            var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
            var tokenGenerator = JwtTokenGeneratorBuilder.Build();

            if (!string.IsNullOrEmpty(email))
                readRepositoryBuilder.ExistActiveUserWithEmail(email);

            return new RegisterUserUseCase(readRepositoryBuilder.Build(), writeRepository, mapper, passwordEncrypter, unitOfWork, tokenGenerator);
        }
    }
}
