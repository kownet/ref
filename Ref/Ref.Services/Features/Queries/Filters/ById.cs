﻿using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
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

        public class Result : BaseResult
        {
            public FilterResult Filter { get; set; }
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

                    if (!(filter is null))
                    {
                        return new Result
                        {
                            Filter = new FilterResult
                            {
                                Id = filter.Id,
                                CityId = filter.CityId,
                                FlatAreaFrom = filter.FlatAreaFrom,
                                FlatAreaTo = filter.FlatAreaTo,
                                PriceFrom = filter.PriceFrom,
                                PriceTo = filter.PriceTo,
                                UserId = filter.UserId,
                                Name = filter.Name,
                                Notification = filter.Notification,
                                ShouldContain = filter.ShouldContain,
                                ShouldNotContain = filter.ShouldNotContain
                            }
                        };
                    }
                    else
                    {
                        return new Result { Message = "No such object" };
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