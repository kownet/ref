using Dapper;
using MediatR;
using Newtonsoft.Json;
using Ref.Data.Db;
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
            public string Location { get; set; }
            public string LocationRaw => string.IsNullOrWhiteSpace(Location)
                ? string.Empty
                : Location.ToLowerInvariant().RemoveDiacritics();
            public bool HasLocationFilter => !string.IsNullOrWhiteSpace(Location);

            public int? PriceFrom { get; set; }
            public bool HasPriceFrom => PriceFrom.HasValue;

            public int? PriceTo { get; set; }
            public bool HasPriceTo => PriceTo.HasValue;

            public bool HasFilter => HasLocationFilter || HasPriceFrom || HasPriceTo;
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
                Offers = new HashSet<OfferResult>();
            }

            public Pagination Pagination { get; set; }
            public IEnumerable<OfferResult> Offers { get; set; }

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
                        IEnumerable<OfferResult> offers = null;
                        Pagination pagination = null;

                        string join = " INNER JOIN Cities C ON O.CityId = C.Id";
                        string sql = "SELECT O.Id, C.Name, O.SiteOfferId, O.SiteType, O.Url, O.Header, O.Price, O.DateAdded" +
                            $" FROM Offers O {join}";

                        string filterSql = string.Empty;

                        if (request.HasFilter)
                        {
                            if (request.HasLocationFilter)
                            {
                                filterSql += " C.NameRaw = @LocationRaw ";
                            }

                            if (request.HasPriceFrom)
                            {
                                filterSql = filterSql.SqlAnd();
                                filterSql += " O.Price >= @PriceFrom ";
                            }

                            if (request.HasPriceTo)
                            {
                                filterSql = filterSql.SqlAnd();
                                filterSql += $" O.Price <= @PriceTo ";
                            }

                            sql += $" WHERE {filterSql}";
                        }

                        if (request.PageNumber.HasValue)
                        {
                            sql += @" ORDER BY O.Id 
                            OFFSET @PageSize * (@PageNumber - 1) ROWS
                            FETCH NEXT @PageSize ROWS ONLY";

                            sql += $" SELECT [TotalCount] = COUNT(*) FROM Offers O {join} ";

                            if (!string.IsNullOrWhiteSpace(filterSql))
                            {
                                sql += $" WHERE {filterSql}";
                            }
                        }

                        using (GridReader results = c.QueryMultiple(sql, request))
                        {
                            offers = await results.ReadAsync<OfferResult>();

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