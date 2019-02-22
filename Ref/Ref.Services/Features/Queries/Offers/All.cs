using Dapper;
using MediatR;
using Newtonsoft.Json;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Ref.Services.Features.Queries.Offers
{
    public class All
    {
        public class Query : PaginableQuery, IRequest<Result>
        {
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public Container Response { get; set; }
        }

        public class Container
        {
            public Container()
            {
                Offers = new HashSet<Offer>();
            }

            public Pagination Pagination { get; set; }
            public IEnumerable<Offer> Offers { get; set; }

            public string XPagination => Pagination != null
                ? JsonConvert.SerializeObject(Pagination)
                : string.Empty;

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
                        int? totalRecords = null;
                        IEnumerable<Offer> offers = null;
                        Pagination pagination = null;

                        string sql = @"SELECT Id, CityId, SiteOfferId, SiteType, Url, Header, Price, DateAdded FROM Offers";

                        if (request.PageNumber.HasValue)
                        {
                            sql += @" ORDER BY Offers.Id 
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

                            sql += " SELECT [TotalCount] = COUNT(*) FROM Offers";
                        }

                        using (GridReader results = c.QueryMultiple(sql, request))
                        {
                            offers = await results.ReadAsync<Offer>();

                            if (request.PageNumber.HasValue)
                            {
                                totalRecords = results.ReadSingle<int>();
                            }
                        }

                        if (request.PageNumber.HasValue)
                        {
                            pagination = new Pagination()
                            {
                                PageNumber = request.PageNumber.Value,
                                PageSize = request.PageSize
                            };

                            pagination.TotalRecords = totalRecords.Value;
                        }

                        if (offers.AnyAndNotNull())
                        {
                            return new Result
                            {
                                Response = new Container
                                {
                                    Pagination = pagination,
                                    Offers = offers
                                }
                            };
                        }

                        return new Result
                        {
                            Message = "There no offers"
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