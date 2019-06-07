using Dapper;
using MediatR;
using Ref.Data.Db;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Filters
{
    public class AllByUserId
    {
        public class Query : IRequest<Result>
        {
            public int UserId { get; set; }
        }

        public class Result : BaseResult
        {
            public Result()
            {
                Filters = new HashSet<FilterResult>();
            }

            public IEnumerable<FilterResult> Filters { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IDbAccess _dbAccess;

            public Handler(IDbAccess dbAccess)
            {
                _dbAccess = dbAccess;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    using (var c = _dbAccess.Connection)
                    {
                        var entities = await c.QueryAsync<FilterResult>(@"SELECT F.Id, F.UserId, F.Name, C.Name as City, D.Name as District, F.FlatAreaFrom, F.FlatAreaTo, F.PriceFrom, F.PriceTo, F.Notification, F.LastCheckedAt, F.ShouldContain, F.ShouldNotContain, F.PricePerMeterFrom, F.PricePerMeterTo, F.DistrictId, F.Property  
                                        FROM Filters F
                                        INNER JOIN Cities C on F.CityId = C.Id
                                        LEFT JOIN Districts D on F.DistrictId = D.Id 
                                        WHERE F.UserId = @UserId", new { request.UserId });

                        return entities.AnyAndNotNull()
                            ? new Result { Filters = entities }
                            : new Result();
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