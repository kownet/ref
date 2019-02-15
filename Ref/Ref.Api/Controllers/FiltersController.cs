﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Services.Features.Queries.Filters;
using System.Threading.Tasks;

namespace Ref.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FiltersController : BaseController
    {
        public FiltersController(
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
                return Ok(result.Filters);
            else
                return BadRequest(result.Message);
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetAllByUser()
        {
            var result = await Mediator.Send(new All.Query { UserId = UserId });

            if (result.Succeed)
                return Ok(result.Filters);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new ById.Query(UserId, id));

            if (result.Succeed)
                return Ok(result.Filter);
            else
                return BadRequest(result.Message);
        }
    }
}