using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;

namespace Ref.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
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

        protected int UserId
        {
            get
            {
                int result = 0;

                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    int.TryParse(User.Identity.Name, out result);
                }

                return result;
            }
        }
    }
}