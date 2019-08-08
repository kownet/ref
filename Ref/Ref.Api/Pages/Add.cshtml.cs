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
        public bool Succeed { get; set; }
        public ErrorAddViewModel ErrorAddViewModel { get; set; }

        public async Task OnGetAsync(string guid)
        {
            Guid = guid;

            var result = await _mediator.Send(new Verify.Query { Guid = Guid });

            Succeed = result.Succeed;

            if (result.Succeed)
            {
                _logger.LogInformation($"Verification on AddPage OK for GUID: {Guid}");

                UserId = result.UserId;

                ErrorAddViewModel = new ErrorAddViewModel
                {
                    IsDemo = result.SubscriptionType == SubscriptionType.Demo,
                    UserGuid = Guid
                };
            }
            else
            {
                _logger.LogError($"{result.Message}, GUID: {guid}");

                ErrorAddViewModel = new ErrorAddViewModel
                {
                    Message = result.Message,
                    IsException = true,
                    UserGuid = guid
                };
            }
        }

        [BindProperty]
        public FilterViewModel FilterViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _mediator.Send(new Create.Cmd
            {
                Name = FilterViewModel.Name,
                Property = (PropertyType)FilterViewModel.Property,
                CityId = FilterViewModel.City,
                DistrictId = FilterViewModel.District,
                Notification = (NotificationType)FilterViewModel.Notification,
                FlatAreaFrom = FilterViewModel.AreaFrom,
                FlatAreaTo = FilterViewModel.AreaTo,
                PriceFrom = FilterViewModel.PriceFrom,
                PriceTo = FilterViewModel.PriceTo,
                PricePerMeterFrom = FilterViewModel.PpmFrom,
                PricePerMeterTo = FilterViewModel.PpmTo,
                ShouldContain = FilterViewModel.ShouldContain,
                ShouldNotContain = FilterViewModel.ShouldNotContain,
                UserId = FilterViewModel.UserId,
                AllowFromAgency = FilterViewModel.AllowFromAgency
            });

            if (!result.Succeed)
            {
                _logger.LogError($"{result.Message}, GUID: {FilterViewModel.UserGuid}");

                ErrorAddViewModel = new ErrorAddViewModel
                {
                    Message = result.Message,
                    IsException = true,
                    IsTooMuch = result.TooMuch,
                    UserGuid = FilterViewModel.UserGuid
                };

                return Page();
            }
                

            return RedirectToPage("Index", new { guid = FilterViewModel.UserGuid });
        }
    }
}