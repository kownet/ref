using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Api.ViewModels;
using Ref.Data.Models;
using Ref.Services.Features.Queries.Users;
using Ref.Shared.Extensions;
using Ref.Shared.Notifications;
using System.Threading.Tasks;

namespace Ref.Api.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;
        private readonly IMediator _mediator;

        private readonly IEmailNotification _emailNotification;

        public ContactModel(
            ILogger<ContactModel> logger,
            IMediator mediator,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _mediator = mediator;
            _emailNotification = emailNotification;
        }

        public string Guid { get; set; }
        public ReasonType ReasonType { get; set; }
        public bool Succeed { get; set; }
        public ErrorContactViewModel ErrorContactViewModel { get; set; }

        public async Task OnGetAsync(string guid, int reasontype)
        {
            Guid = guid;
            ReasonType = (ReasonType)reasontype;

            var result = await _mediator.Send(new Verify.Query { Guid = Guid });

            Succeed = result.Succeed;

            if (result.DemoPassed || result.Succeed)
            {
                Succeed = true;

                ContactViewModel = new ContactViewModel
                {
                    Email = result.Email,
                    UserGuid = Guid,
                    Subject = ReasonType.GetDescription(),
                    Message = string.Empty
                };
            }
            else
            {
                ErrorContactViewModel = new ErrorContactViewModel
                {
                    IsException = true,
                    Message = result.Message,
                    UserGuid = guid
                };

                _logger.LogError($"{result.Message}, GUID: {guid}");
            }
        }

        [BindProperty]
        public ContactViewModel ContactViewModel { get; set; }
        public bool Sent { get; set; }

        public IActionResult OnPost()
        {
            Guid = ContactViewModel.UserGuid;

            var emailSent = _emailNotification.Send(
                $"[PM] {ContactViewModel.Subject}",
                $"{ContactViewModel.Email} pisze: {ContactViewModel.Message}",
                $"{ContactViewModel.Email} pisze: {ContactViewModel.Message}",
                new string[] { "tkowalczyk.poczta@gmail.com" }
                );

            if (!emailSent.IsSuccess)
            {
                ErrorContactViewModel = new ErrorContactViewModel
                {
                    IsException = true,
                    Message = emailSent.Message,
                    UserGuid = Guid
                };

                _logger.LogError($"{emailSent.Message}, GUID: {Guid}");
            }
            else
            {
                Sent = true;
            }

            return Page();
        }
    }
}