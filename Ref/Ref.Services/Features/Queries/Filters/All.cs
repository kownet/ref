using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
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
                Filters = new HashSet<FilterResult>();
            }

            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public IEnumerable<FilterResult> Filters { get; set; }
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
                    var filters = request.UserId.HasValue
                        ? await _filterRepository.FindByAsync(f => f.UserId == request.UserId.Value)
                        : await _filterRepository.GetAllAsync();

                    if (filters.AnyAndNotNull())
                    {
                        var filterResult = new List<FilterResult>();

                        foreach (var filter in filters)
                        {
                            filterResult.Add(new FilterResult
                            {
                                Id = filter.Id,
                                CityId = filter.CityId,
                                FlatAreaFrom = filter.FlatAreaFrom,
                                FlatAreaTo = filter.FlatAreaTo,
                                PriceFrom = filter.PriceFrom,
                                PriceTo = filter.PriceTo,
                                UserId = filter.UserId,
                                Name = filter.Name,
                                Notification = filter.Notification
                            });
                        }

                        return new Result
                        {
                            Filters = filterResult
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Message = "There are no filters"
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