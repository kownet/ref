using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Filters
{
    public class ById
    {
        public class Query : IRequest<Result>
        {
            public Query(int userId, int filterId)
            {
                UserId = userId;
                FilterId = filterId;
            }

            public int UserId { get; private set; }
            public int FilterId { get; private set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public Filter Filter { get; set; }
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
                    var filter = (await _filterRepository
                        .FindByAsync(f
                            => f.Id == request.FilterId && f.UserId == request.UserId))
                        .FirstOrDefault();

                    if (filter != null)
                    {
                        return new Result { Filter = filter };
                    }
                    else
                    {
                        return new Result
                        {
                            Message = "No such object"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}