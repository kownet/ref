using Dapper;
using MediatR;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Poc
{
    public class Filters
    {
        public class Query : IRequest<Result>
        {
            public int UserId { get; set; }
        }

        public class Result : BaseResult
        {
            public Result()
            {
                Filters = new HashSet<FilterPoc>();
            }

            public IEnumerable<FilterPoc> Filters { get; set; }
        }

        public class FilterPoc
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public int FlatAreaFrom { get; set; }
            public int FlatAreaTo { get; set; }
            public int PriceFrom { get; set; }
            public int PriceTo { get; set; }
            public NotificationType Notification { get; set; }
            public string NotificationFormatted => Notification.GetDescription();
            public DateTime? LastCheckedAt { get; set; }
            public string LastCheckedAtFormatted => LastCheckedAt.Format("niesprawdzany");
            public string ShouldContain { get; set; }
            public string ShouldNotContain { get; set; }
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
                        var entities = await c.QueryAsync<FilterPoc>(@"SELECT F.Id, F.UserId, F.Name, C.Name as City, F.FlatAreaFrom, F.FlatAreaTo, F.PriceFrom, F.PriceTo, F.Notification, F.LastCheckedAt, F.ShouldContain, F.ShouldNotContain 
                                        FROM Filters F
                                        INNER JOIN Cities C on F.CityId = C.Id
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