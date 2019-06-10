using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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

        public async Task OnGetAsync(string guid, int id)
        {
            Guid = guid;
            Id = id;

            var result = await _mediator.Send(new FilterById.Query(Guid, Id));

            if (result.Succeed)
            {
                _logger.LogInformation($"Filter found: {Id}");


            }
            else
                _logger.LogError(result.Message);
        }
    }
}