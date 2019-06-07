﻿using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Api.ViewModels;
using Ref.Services.Features.Queries.Users;
using System.Threading.Tasks;

namespace Ref.Api.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMediator _mediator;

        public IndexModel(
            ILogger<IndexModel> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public string Guid { get; set; }

        public IndexViewModel IndexViewModel { get; set; }

        public async Task OnGetAsync(string guid)
        {
            Guid = guid;

            var result = await _mediator.Send(new Verify.Query { Guid = Guid });

            if (result.Succeed)
            {
                _logger.LogInformation($"Verification OK for GUID: {Guid}");

                IndexViewModel = new IndexViewModel
                {
                    UserId = result.UserId,
                    RegisteredAt = result.RegisteredAt,
                    Email = result.Email,
                    IsActive = result.IsActive
                };
            }
            else
                _logger.LogError(result.Message);
        }
    }
}