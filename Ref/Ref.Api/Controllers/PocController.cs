using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
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

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(Verify.Query q)
        {
            var result = await Mediator.Send(q);

            if (!result.Succeed)
                _logger.LogError(result.Message);

            return Ok(result);
        }

        [HttpPost("email")]
        public async Task<IActionResult> Email(Email.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (!result.Succeed)
                _logger.LogError(result.Message);

            return Ok(result);
        }
    }
}