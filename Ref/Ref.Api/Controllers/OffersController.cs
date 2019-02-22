using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public OffersController(
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(All.Query q)
        {
            var result = await Mediator.Send(q);

            if (result.Succeed)
            {
                Response.Headers.Add("X-Pagination", result.Response.XPagination);

                return Ok(result.Response.Offers);
            }
            else
                return BadRequest(result.Message);
        }
    }
}