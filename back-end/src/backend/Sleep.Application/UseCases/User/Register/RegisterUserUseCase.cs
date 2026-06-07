using AutoMapper;
using Sleep.Application.Services.Cryptography;
using System.ComponentModel.DataAnnotations;
using Sleep.Application.Tokens;
using Sleep.Communication.Responses;
using Sleep.Communication.Responses.Token;
using Sleep.Domain.Entities;
using Sleep.Domain.Enum;
using Sleep.Domain.Repositories;
using Sleep.Domain.Repositories.User;
using Sleep.Domain.Security.Cryptography;
using Sleep.Exceptions;
using Sleep.Exceptions.ExceptionsBase;
using Sleep.Communication.Requests.User;

namespace Sleep.Application.UseCases.User.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepositoryReadOnly _readRepository;
        private readonly IUserRepositoryWriteOnly _writeRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordEncrypt _passwordEncrypter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessTokenGenerator _tokenAccess;

        public RegisterUserUseCase(
            IUserRepositoryReadOnly readRepository, 
            IUserRepositoryWriteOnly writeRepository, 
            IMapper mapper,
            IPasswordEncrypt passwordEncrypter, 
            IUnitOfWork unitOfWork,
            IAccessTokenGenerator accessTokenGenerator)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _mapper = mapper;
            _passwordEncrypter = passwordEncrypter;
            _unitOfWork = unitOfWork;
            _tokenAccess = accessTokenGenerator;
        }

        public async Task<ResponseUserRegister> Execute(RequestRegisterUserJson request)
        {
            await Validate(request);
            
            var user = _mapper.Map<Domain.Entities.User>(request);
            user.Password = _passwordEncrypter.Encrypt(request.Password);
            user.UserIdentifier = Guid.NewGuid();
            user.Status = UserStatus.Active;
            
            await _writeRepository.Add(user);
            await _unitOfWork.Commit();

            return new ResponseUserRegister
            {
                Name = user.Name,
                Tokens = new ResponseTokenJson
                {
                    AccessToken = _tokenAccess.Generate(user.UserIdentifier)
                }
            };
        }

        private async Task Validate(RequestRegisterUserJson request)
        {
            var validator = new RegisterUserValidator();
            var result = validator.Validate(request);

            var emailExist = await _readRepository.ExistUserWithEmail(request.Email);

            if (emailExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, ResourceMessageException.EMAIL_ALREADY_REGISTERED));

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
