using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;

namespace Ref.Api.Controllers
{
    [ApiController]
    public class BaseController : Controller
    {
        protected readonly IMediator Mediator;
        protected readonly AppSettings Settings;

        public BaseController(
            IMediator mediator,
            IOptions<AppSettings> appSettings)
        {
            Mediator = mediator;
            Settings = appSettings.Value;
        }
    }
}