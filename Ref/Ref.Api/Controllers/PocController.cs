using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Commands.Filters;
using Ref.Services.Features.Commands.Poc;
using Ref.Services.Features.Queries.Poc;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PocController : BaseController
    {
        private readonly ILogger<PocController> _logger;

        public PocController(
            ILogger<PocController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        //[HttpPost("verify")]
        //public async Task<IActionResult> Verify(Verify.Query q)
        //{
        //    var result = await Mediator.Send(q);

        //    if (!result.Succeed)
        //        _logger.LogError(result.Message);
        //    else
        //        _logger.LogInformation($"Verification OK for GUID: {q.Guid}");

        //    return Ok(result);
        //}

        //[HttpPost("email")]
        //public async Task<IActionResult> Email(Email.Cmd cmd)
        //{
        //    var result = await Mediator.Send(cmd);

        //    if (!result.Succeed)
        //        _logger.LogError(result.Message);
        //    else
        //        _logger.LogInformation($"Changing email OK for user: {cmd.Id}");

        //    return Ok(result);
        //}

        [HttpPost("filters")]
        public async Task<IActionResult> Filters(Filters.Query q)
        {
            var result = await Mediator.Send(q);

            if (!result.Succeed)
                _logger.LogError(result.Message);

            return Ok(result);
        }

        [HttpDelete("deletefilter/{id}")]
        public async Task<IActionResult> DeleteFilter(int id)
        {
            var cmd = new DeleteFilter.Cmd(id);

            var result = await Mediator.Send(cmd);

            if (!result.Succeed)
                _logger.LogError(result.Message);
            else
                _logger.LogInformation($"Deleting filter OK: {id}");

            return Ok(result);
        }

        [HttpPost("addfilter")]
        public async Task<IActionResult> AddFilter(Create.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (!result.Succeed)
                _logger.LogError(result.Message);
            else
                _logger.LogInformation($"Creating filter OK for user: {cmd.UserId}, filter: {cmd.Name}");

            return Ok(result);
        }

        [HttpGet("getfilter/{id}")]
        public async Task<IActionResult> GetFilter(int id)
        {
            var q = new FilterById.Query(id);

            var result = await Mediator.Send(q);

            if (!result.Succeed)
                _logger.LogError(result.Message);

            return Ok(result);
        }

        [HttpPut("update")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateFilter(Update.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (!result.Succeed)
                _logger.LogError(result.Message);
            else
                _logger.LogInformation($"Updating filter OK: {cmd.Id}");

            return Ok(result);
        }
    }
}