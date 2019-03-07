using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Data.Models;
using Ref.Services.Features.Commands.Users;
using Ref.Services.Features.Queries.Users;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        public UsersController(
            IOptions<AppSettings> appSettings,
            IMediator mediator)
            : base(mediator, appSettings)
        {
        }

        /// <summary>
        /// Register user in system
        /// </summary>
        /// <param name="cmd">Register user command</param>
        /// <returns>Status code</returns>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Register.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="cmd">Authenticate user command</param>
        /// <returns>Status code</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(Authenticate.Cmd cmd)
        {
            cmd.SigningToken = Settings.Secret;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok(result.Token);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Current user object</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id != UserId)
                return Forbid();

            var result = await Mediator.Send(new ById.Query(UserId));

            if (result.Succeed)
                return Ok(result.User);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Get all users stored in system database (for admin role only)
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new All.Query());

            if (result.Succeed)
                return Ok(result.Users);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Deletes a user by Id (for admin role only)
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>Status code</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var cmd = new Delete.Cmd(id);

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Updates current user (authenticated)
        /// </summary>
        /// <param name="cmd">New user properties</param>
        /// <returns>Status code</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(Update.Cmd cmd)
        {
            cmd.Id = UserId;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// Resets user password (for admin role only)
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="cmd">Reset password command</param>
        /// <returns>Status code</returns>
        [HttpPut("reset/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Reset(int id, Reset.Cmd cmd)
        {
            cmd.Id = id;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        [HttpPost("lost")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [AllowAnonymous]
        public async Task<IActionResult> Lost(Lost.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }
    }
}