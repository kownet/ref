using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Queries.Districts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DistrictsController : BaseController
    {
        private readonly ILogger<DistrictsController> _logger;

        public DistrictsController(
            ILogger<DistrictsController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all districts for city
        /// </summary>
        /// <returns>Districts for city</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("city/{id}")]
        public async Task<IActionResult> GetAllByCity(int id)
        {
            var result = await Mediator.Send(new ByCityId.Query(id));

            if (result.Succeed)
                return Ok(result.Districts);
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }
    }
}