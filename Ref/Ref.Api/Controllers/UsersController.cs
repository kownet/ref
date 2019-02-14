using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Commands.Users;
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register.Cmd cmd)
        {
            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok();
            else
                return BadRequest(result.Message);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(Authenticate.Cmd cmd)
        {
            cmd.SigningToken = Settings.Secret;

            var result = await Mediator.Send(cmd);

            if (result.Succeed)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}