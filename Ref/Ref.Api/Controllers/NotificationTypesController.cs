﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Data.Models;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Linq;

namespace Ref.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationTypesController : BaseController
    {
        private readonly ILogger<NotificationTypesController> _logger;

        public NotificationTypesController(
            ILogger<NotificationTypesController> logger,
            IMediator mediator,
            IOptions<AppSettings> appSettings)
            : base(mediator, appSettings)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all notifications types
        /// </summary>
        /// <returns>All notification types stored in system</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetAll()
        {
            return Ok(Enum.GetValues(typeof(NotificationType))
                       .Cast<NotificationType>()
                       //.Except(new NotificationType[] { NotificationType.Undefinded })
                       .Select(t => new NotificationTypeResult
                       {
                           Id = ((int)t),
                           Name = t.ToString(),
                           NamePl = t.GetDescription()
                       }));
        }
    }
}