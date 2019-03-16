using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Data.Models;
using Ref.Services.Features.Commands.Filters;
using Ref.Services.Features.Queries.Filters;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FiltersController : BaseController
    {
        private readonly ILogger<FiltersController> _logger;

        public FiltersController(
            ILogger<FiltersController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all filters (for admin role only)
        /// </summary>
        /// <returns>All filters stored in system database</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new All.Query());

            if (result.Succeed)
                return Ok(result.Filters);
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Get all filters for current user (authenticated)
        /// </summary>
        /// <returns>Current user filters</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Route("user")]
        public async Task<IActionResult> GetAllByUser()
        {
            var result = await Mediator.Send(new All.Query { UserId = UserId });

            if (result.Succeed)
                return Ok(result.Filters);
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Get filter by Id for current user (authenticated)
        /// </summary>
        /// <param name="id">Filter id</param>
        /// <returns>Filter for current user (authenticated)</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new ById.Query(UserId, id));

            if (result.Succeed)
                return Ok(result.Filter);
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Creates a filter
        /// </summary>
        /// <param name="cmd">Create user command</param>
        /// <returns>Status code</returns>
        [HttpPost("create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Create.Cmd cmd)
        {
            cmd.UserId = UserId;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Deletes a filter by Id for current user (authenticated)
        /// </summary>
        /// <param name="id">Filter id</param>
        /// <returns>Status code</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(int id)
        {
            var cmd = new Delete.Cmd(id, UserId);

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Updates a filter by Id for current user (authenticated)
        /// </summary>
        /// <param name="id">Filter id</param>
        /// <param name="cmd">New filter properties command</param>
        /// <returns>Status code</returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        //[Route("user")]
        public async Task<IActionResult> Update(int id, Update.Cmd cmd)
        {
            cmd.Id = id;
            cmd.UserId = UserId;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
            {
                _logger.LogError(result.Message);

                return BadRequest(result.Message);
            }
        }
    }
}