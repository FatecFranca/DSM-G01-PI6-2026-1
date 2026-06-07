using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Sleep.Application.Services.LoggedUser;
using Sleep.Application.Tokens;
using Sleep.Domain.Entities;
using Sleep.Domain.Enum;
using Sleep.Infrasctucture.DataAccess;

namespace Sleep.Infrasctructure.Services.LoggedUser
{
    public class LoggedUser : ILoggedUser
    {

        private readonly SleepDbContext _db;
        private readonly ITokenProvider _tokenProvider;

        public LoggedUser(SleepDbContext db, ITokenProvider tokenProvider)
        {
            _db = db;
            _tokenProvider = tokenProvider;
        }

        public async Task<User> User()
        {
            var token = _tokenProvider.Value();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            string identifier = jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

            var userIdentifer = Guid.Parse(identifier);

            return await _db
                .Users
                .AsNoTracking()
                .FirstAsync(user => user.Status == UserStatus.Active && user.UserIdentifier == userIdentifer);
        }
    }
}
