﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Queries.Cities;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitiesController : BaseController
    {
        public CitiesController(
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new All.Query());

            if (result.Succeed)
                return Ok(result.Cities);
            else
                return BadRequest(result.Message);
        }
    }
}