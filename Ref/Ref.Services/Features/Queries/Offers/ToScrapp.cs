using Dapper;
using MediatR;
using Ref.Data.Db;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Offers
{
    public class ToScrapp
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result : BaseResult
        {
            public Result()
            {
                Offers = new HashSet<ToScrappResult>();
            }

            public IEnumerable<ToScrappResult> Offers { get; set; }
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
                        var result = await c.QueryAsync<ToScrappResult>(
                             @"SELECT count(*) AS ToScrapp, Site FROM Offers WHERE IsScrapped = 0 GROUP BY Site ORDER BY Site");

                        if (result.AnyAndNotNull())
                        {
                            return new Result { Offers = result };
                        }
                        else
                        {
                            return new Result { Message = "Nothing to scrapp." };
                        }
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