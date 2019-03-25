using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Queries.AdminInfos;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminInfosController : BaseController
    {
        private readonly ILogger<AdminInfosController> _logger;

        public AdminInfosController(
            ILogger<AdminInfosController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all admin infos
        /// </summary>
        /// <returns>All active admin infos stored in system database</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new All.Query());

            if (result.Succeed)
                return Ok(result.Infos);
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }
    }
}