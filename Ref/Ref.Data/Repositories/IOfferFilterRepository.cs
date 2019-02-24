using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IOfferFilterRepository : IRepository
    {
        void BulkInsert(IList<OfferFilter> offers, bool withUpdate = false);
        Task<IQueryable<OfferFilter>> FindByAsync(Expression<Func<OfferFilter, bool>> predicate);
    }

    public class OfferFilterRepository : IOfferFilterRepository
    {
        private readonly IDbAccess _dbAccess;

        public OfferFilterRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void BulkInsert(IList<OfferFilter> offers, bool withUpdate = false)
        {
            using (var c = _dbAccess.Connection)
            {
                using (var trans = c.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (var sbc = new SqlBulkCopy((SqlConnection)c, SqlBulkCopyOptions.Default, (SqlTransaction)trans))
                    {
                        sbc.BatchSize = offers.Count;
                        sbc.DestinationTableName = "dbo.OfferFilters";

                        var dt = offers.ToDataTable();

                        sbc.ColumnMappings.Add("OfferId", "OfferId");
                        sbc.ColumnMappings.Add("FilterId", "FilterId");
                        sbc.ColumnMappings.Add("Sent", "Sent");

                        sbc.WriteToServer(dt);
                    }

                    if (withUpdate)
                    {
                        var chunked = offers.Chunk(1000);

                        foreach (var chunk in chunked)
                        {
                            c.Execute(
                                    @"UPDATE Filters SET LastCheckedAt = @LastCheckedAt WHERE Id IN @Ids",
                                    new
                                    {
                                        Ids = chunk.Select(f => f.FilterId),
                                        LastCheckedAt = DateTime.Now
                                    }, trans);
                        }
                    }

                    trans.Commit();
                }
            }
        }

        public async Task<IQueryable<OfferFilter>> FindByAsync(Expression<Func<OfferFilter, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<OfferFilter>(
                    @"SELECT OfferId, FilterId, Sent FROM OfferFilters")).AsQueryable();

                return result.Where(predicate);
            }
        }
    }
}