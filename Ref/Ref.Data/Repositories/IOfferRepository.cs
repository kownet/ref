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
    public interface IOfferRepository : IRepository
    {
        Task<IQueryable<Offer>> FindByAsync(Expression<Func<Offer, bool>> predicate);
        void BulkInsert(IList<Offer> offers);
        void BulkDelete(IList<int> offers);
        Task<int> UpdateAsync(Offer offer);
        Task<int> SetDeletedAsync(int offerId);
        Task<int> SetDistrict(int offerId, int districtId);
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
                using (var trans = c.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    c.Execute("DELETE FROM OfferFilters WHERE OfferId IN @Ids",
                        new
                        {
                            Ids = offers.ToArray()
                        }, trans);

                    c.Execute("DELETE FROM Offers WHERE Id IN @Ids",
                        new
                        {
                            Ids = offers.ToArray()
                        }, trans);

                    trans.Commit();
                }
            }
        }

        public void BulkInsert(IList<Offer> offers)
        {
            offers.Change(o => o.Url = o.Url.Truncate(254));
            offers.Change(o => o.Header = o.Header.Truncate(254));

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
                        sbc.ColumnMappings.Add("Site", "Site");
                        sbc.ColumnMappings.Add("Deal", "Deal");
                        sbc.ColumnMappings.Add("Url", "Url");
                        sbc.ColumnMappings.Add("Header", "Header");
                        sbc.ColumnMappings.Add("Price", "Price");
                        sbc.ColumnMappings.Add("Area", "Area");
                        sbc.ColumnMappings.Add("Rooms", "Rooms");
                        sbc.ColumnMappings.Add("PricePerMeter", "PricePerMeter");
                        sbc.ColumnMappings.Add("DateAdded", "DateAdded");
                        sbc.ColumnMappings.Add("IsScrapped", "IsScrapped");
                        sbc.ColumnMappings.Add("Floor", "Floor");
                        sbc.ColumnMappings.Add("Content", "Content");
                        sbc.ColumnMappings.Add("DistrictId", "DistrictId");

                        sbc.WriteToServer(dt);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        _dbAccess.CloseConnection();
                    }
                }
            }
        }

        public async Task<IQueryable<Offer>> FindByAsync(Expression<Func<Offer, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Offer>(
                    @"SELECT Id, CityId, SiteOfferId, Site, Deal, Url, Header, Price, DateAdded, Area, Rooms, PricePerMeter, IsScrapped, Floor, Content, DistrictId FROM Offers")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<int> SetDeletedAsync(int offerId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Offers SET IsScrapped = @IsScrapped WHERE Id = @Id",
                    new
                    {
                        Id = offerId,
                        IsScrapped = true
                    });
            }
        }

        public async Task<int> SetDistrict(int offerId, int districtId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Offers SET DistrictId = @DistrictId WHERE Id = @Id",
                    new
                    {
                        Id = offerId,
                        DistrictId = districtId
                    });
            }
        }

        public async Task<int> UpdateAsync(Offer offer)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Offers SET Content = @Content, Floor = @Floor, IsScrapped = @IsScrapped, Area = @Area, Rooms = @Rooms, PricePerMeter = @PricePerMeter WHERE Id = @Id",
                    new
                    {
                        offer.Id,
                        offer.Content,
                        offer.Floor,
                        offer.IsScrapped,
                        offer.Area,
                        offer.Rooms,
                        offer.PricePerMeter
                    });
            }
        }
    }
}