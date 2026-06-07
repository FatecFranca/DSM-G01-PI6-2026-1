using Microsoft.AspNetCore.Mvc;
using Sleep.Api.Attributes;
using Sleep.Application.UseCases.Sleep.Anlysis;
using Sleep.Application.UseCases.Sleep.Create;
using Sleep.Application.UseCases.Sleep.Get.Analysis;
using Sleep.Application.UseCases.Sleep.Get.ById;
using Sleep.Application.UseCases.Sleep.Get.SleepHistory;
using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Utils.Page;
using System.Security.Cryptography;

namespace Sleep.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SleepController : ControllerBase
    {
        [HttpPost]
        [AuthenticadedUser]
        public async Task<IActionResult> Register(
            [FromBody] RequestCreateSleepRecord request,
            [FromServices] ICreateSleepRecordUseCase useCase
            )
        {
            await useCase.Execute(request);
            return Created();
        }


        [HttpGet]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(PagedList<ShortSleepRecord>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSleepHistory(
                [FromQuery] PageParameters pageParameters,
                [FromQuery] RequestSleepHistoryFilter requestFilter,
                [FromServices] IGetSleepHistoryUseCase useCase
            )
        {
            var result = await useCase.Execute(pageParameters, requestFilter);
            return Ok(result);
        }

        [HttpGet("{recordId}")]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(ResponseSleepRecordDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSleepRecordById(
            [FromRoute] long recordId,
            [FromServices] IGetSleepRecordByIdUseCase useCase)
        {
            var response = await useCase.Execute(recordId);
            return Ok(response);
        }

        [HttpGet("{recordId}/analysis")]
        [AuthenticadedUser]
        [ProducesResponseType(typeof(ResponseSleepAnalysis), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSleepAnalysis(
            [FromRoute] long recordId,
            [FromServices] IGetSleepAnalysisUseCase useCase)
        {
            var response = await useCase.Execute(recordId);
            return Ok(response);
        }

        [HttpPost("{recordId}/analysis")]
        [AuthenticadedUser]
        public async Task<IActionResult> DoSleepAnalysis(
            [FromServices] IDoSleepAnalysisUseCase useCase,
            [FromRoute] long recordId
            )
        {
            await useCase.Execute(recordId);
            return NoContent();
        }

    }
}
