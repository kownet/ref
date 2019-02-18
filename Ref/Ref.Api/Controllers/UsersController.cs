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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Register.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        [HttpPost("authenticate")]
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

        [HttpGet("{id}")]
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

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new All.Query());

            if (result.Succeed)
                return Ok(result.Users);
            else
                return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Update.Cmd cmd)
        {
            cmd.Id = UserId;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        [HttpPut("reset/{id}")]
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
    }
}