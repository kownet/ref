using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Filters
{
    public class All
    {
        public class Query : IRequest<Result>
        {
            public int? UserId { get; set; }
        }

        public class Result
        {
            public Result()
            {
                Filters = new HashSet<Filter>();
            }

            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public IEnumerable<Filter> Filters { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IFilterRepository _filterRepository;

            public Handler(IFilterRepository filterRepository)
            {
                _filterRepository = filterRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    return new Result
                    {
                        Filters = request.UserId.HasValue
                        ? await _filterRepository.FindByAsync(f => f.UserId == request.UserId.Value)
                        : await _filterRepository.GetAll()
                    };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}