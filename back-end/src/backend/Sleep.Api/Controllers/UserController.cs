using Microsoft.AspNetCore.Mvc;
using Sleep.Api.Attributes;
using Sleep.Application.UseCases.User.Profile.Get;
using Sleep.Application.UseCases.User.Profile.Update;
using Sleep.Application.UseCases.User.Register;
using Sleep.Communication.Requests.User;
using Sleep.Communication.Responses;
using Sleep.Communication.Responses.User;

namespace Sleep.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserRegister), StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterUserUseCase useCase,
            [FromBody]RequestRegisterUserJson requestBody
            )
        {
           var result = await useCase.Execute(requestBody);

           return Created(string.Empty, result);
        }

        [HttpGet]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(ResponseUserProfile), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserProfile(
            [FromServices] IGetUserProfileUseCase useCase
            )
        {
            var response = await useCase.Execute();
            return Ok(response);
        }

        [HttpPut]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(ResponseUserProfile), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserProfile(
            [FromServices] IUpdateUserProfileUseCase useCase,
            [FromBody] RequestUpdateUserJson requestBody)
        {
            var response = await useCase.Execute(requestBody);
            return Ok(response);
        }
    }
}
