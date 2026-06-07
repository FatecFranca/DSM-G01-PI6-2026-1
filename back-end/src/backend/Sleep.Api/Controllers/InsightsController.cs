using Microsoft.AspNetCore.Mvc;
using Sleep.Api.Attributes;
using Sleep.Application.UseCases.Insights.Get;
using Sleep.Communication.Responses.Insights;

namespace Sleep.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InsightsController : ControllerBase
    {
        [HttpGet]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(ResponseInsights), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInsights(
            [FromServices] IGetInsightsUseCase useCase)
        {
            var response = await useCase.Execute();
            return Ok(response);
        }
    }
}
