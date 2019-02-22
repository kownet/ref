using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IOfferRepository : IRepository
    {
        Task<IQueryable<Offer>> FindByAsync(Expression<Func<Offer, bool>> predicate);
        void BulkInsert(IList<Offer> offers);
        void BulkDelete(IList<int> offers);
    }

    public class OfferRepository : IOfferRepository
    {
        private readonly IDbAccess _dbAccess;

        public OfferRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void BulkDelete(IList<int> offers)
        {
            using (var c = _dbAccess.Connection)
            {
                c.Execute("DELETE FROM Offers WHERE Id IN @Ids",
                    new
                    {
                        Ids = offers.ToArray()
                    });
            }
        }

        public void BulkInsert(IList<Offer> offers)
        {
            using (var c = _dbAccess.Connection)
            {
                using (var sbc = new SqlBulkCopy((SqlConnection)c))
                {
                    sbc.BatchSize = offers.Count;
                    sbc.DestinationTableName = "dbo.Offers";
                    try
                    {
                        var dt = offers.ToDataTable();

                        sbc.ColumnMappings.Add("CityId", "CityId");
                        sbc.ColumnMappings.Add("SiteOfferId", "SiteOfferId");
                        sbc.ColumnMappings.Add("SiteType", "SiteType");
                        sbc.ColumnMappings.Add("DealType", "DealType");
                        sbc.ColumnMappings.Add("Url", "Url");
                        sbc.ColumnMappings.Add("Header", "Header");
                        sbc.ColumnMappings.Add("Price", "Price");
                        sbc.ColumnMappings.Add("DateAdded", "DateAdded");

                        sbc.WriteToServer(dt);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public async Task<IQueryable<Offer>> FindByAsync(Expression<Func<Offer, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Offer>(
                    @"SELECT Id, CityId, SiteOfferId, SiteType, Url, Header, Price, DateAdded FROM Offers")).AsQueryable();

                return result.Where(predicate);
            }
        }
    }
}