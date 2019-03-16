using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Queries.Offers;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OffersController : BaseController
    {
        private readonly ILogger<OffersController> _logger;

        public OffersController(
            ILogger<OffersController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all offers
        /// </summary>
        /// <param name="q">Offers query with filter if provided</param>
        /// <returns>All offers stored in system database (filtered if needed)</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll(All.Query q)
        {
            var result = await Mediator.Send(q);

            if (result.Succeed)
            {
                Response.Headers.Add("X-Pagination", result.Response.XPagination);

                return Ok(result.Response.Offers);
            }
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }
    }
}