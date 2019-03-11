using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ref.Api.Helpers;
using Ref.Data.Models;

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

        /// <summary>
        /// Returns the authenticated user identificator
        /// </summary>
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

        protected bool IsAdmin => User.IsInRole(Role.Admin);

        protected bool IsNotAdmin => !IsAdmin;

        protected bool IsUser => User.IsInRole(Role.User);
    }
}