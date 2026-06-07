using Microsoft.AspNetCore.Mvc;
using Sleep.Application.UseCases.Auth.Login;
using Sleep.Communication.Requests.Login;
using Sleep.Communication.Responses;
using Sleep.Communication.Responses.Token;
using System.Runtime.CompilerServices;

namespace Sleep.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseTokenJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] RequestLoginJson requestBody,
            [FromServices] IDoLoginUseCase useCase
            )
        {
            var response = await useCase.Execute(requestBody);
            return Ok(response);
        }
    }
}
