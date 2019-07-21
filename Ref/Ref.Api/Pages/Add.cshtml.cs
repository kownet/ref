using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Api.ViewModels;
using Ref.Data.Models;
using Ref.Services.Features.Commands.Filters;
using Ref.Services.Features.Queries.Users;
using System.Threading.Tasks;

namespace Ref.Api.Pages
{
    public class AddModel : PageModel
    {
        private readonly ILogger<AddModel> _logger;
        private readonly IMediator _mediator;

        public AddModel(
            ILogger<AddModel> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public string Guid { get; set; }
        public int UserId { get; set; }

        public async Task OnGetAsync(string guid)
        {
            Guid = guid;

            var result = await _mediator.Send(new Verify.Query { Guid = Guid });

            if (result.Succeed)
            {
                _logger.LogInformation($"Verification OK for GUID: {Guid}");

                UserId = result.UserId;
            }
            else
                _logger.LogError($"{result.Message}, GUID: {guid}");
        }

        [BindProperty]
        public AddViewModel AddViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _mediator.Send(new Create.Cmd
            {
                Name = AddViewModel.Name,
                Property = (PropertyType)AddViewModel.Property,
                CityId = AddViewModel.City,
                DistrictId = AddViewModel.District,
                Notification = (NotificationType)AddViewModel.Notification,
                FlatAreaFrom = AddViewModel.AreaFrom,
                FlatAreaTo = AddViewModel.AreaTo,
                PriceFrom = AddViewModel.PriceFrom,
                PriceTo = AddViewModel.PriceTo,
                PricePerMeterFrom = AddViewModel.PpmFrom,
                PricePerMeterTo = AddViewModel.PpmTo,
                ShouldContain = AddViewModel.ShouldContain,
                ShouldNotContain = AddViewModel.ShouldNotContain,
                UserId = AddViewModel.UserId
            });

            //if(result.Succeed)
            return RedirectToPage("Index", new { guid = AddViewModel.Guid });
        }
    }
}