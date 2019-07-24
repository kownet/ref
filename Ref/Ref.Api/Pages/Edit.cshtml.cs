using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Api.ViewModels;
using Ref.Services.Features.Queries.Filters;
using System.Threading.Tasks;

namespace Ref.Api.Pages
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly IMediator _mediator;

        public EditModel(
            ILogger<EditModel> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public string Guid { get; set; }
        public int Id { get; set; }
        public bool Succeed { get; set; }
        public ErrorAddViewModel ErrorAddViewModel { get; set; }

        public async Task OnGetAsync(string guid, int id)
        {
            Guid = guid;
            Id = id;

            var result = await _mediator.Send(new FilterById.Query(Guid, Id));

            Succeed = result.Succeed;

            if (result.Succeed)
            {
                _logger.LogInformation($"Filter found: {Id}");

                FilterViewModel = new FilterViewModel
                {
                    Name = result.Filter.Name,
                    AreaFrom = result.Filter.FlatAreaFrom,
                    AreaTo = result.Filter.FlatAreaTo,
                    PriceFrom = result.Filter.PriceFrom,
                    PriceTo = result.Filter.PriceTo,
                    PpmFrom = result.Filter.PricePerMeterFrom,
                    PpmTo = result.Filter.PricePerMeterTo,
                    ShouldContain = result.Filter.ShouldContain,
                    ShouldNotContain = result.Filter.ShouldNotContain
                };
            }
            else
            {
                _logger.LogError(result.Message);

                ErrorAddViewModel = new ErrorAddViewModel
                {
                    Message = result.Message,
                    IsException = true
                };
            }
        }

        [BindProperty]
        public FilterViewModel FilterViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!result.Succeed)
            //    _logger.LogError($"{result.Message}, GUID: {FilterViewModel.Guid}");

            return RedirectToPage("Index", new { guid = FilterViewModel.Guid });
        }
    }
}