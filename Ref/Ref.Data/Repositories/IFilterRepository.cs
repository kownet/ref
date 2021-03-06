﻿using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IFilterRepository : IRepository
    {
        Task<IEnumerable<Filter>> GetAllAsync();
        Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate);
        Task<int> CreateAsync(Filter filter);
        Task<int> DeleteAsync(int filterId, int userId);
        Task<int> DeleteAsync(int filterId);
        Task<int> UpdateAsync(Filter filter);
        Task<Filter> GetAsync(int filterId, int userId);
    }

    public class FilterRepository : IFilterRepository
    {
        private readonly IDbAccess _dbAccess;

        public FilterRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<IEnumerable<Filter>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<Filter>(
                    @"SELECT Id, UserId, Property, Deal, Market, CityId, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt, ShouldContain, ShouldNotContain, PricePerMeterFrom, PricePerMeterTo, DistrictId, AllowPrivate, AllowFromAgency FROM Filters");
            }
        }

        public async Task<IQueryable<Filter>> FindByAsync(Expression<Func<Filter, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Filter>(
                    @"SELECT Id, UserId, Property, Deal, Market, CityId, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt, ShouldContain, ShouldNotContain, PricePerMeterFrom, PricePerMeterTo, DistrictId, AllowPrivate, AllowFromAgency FROM Filters")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<int> CreateAsync(Filter filter)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Filters (UserId, Property, Deal, Market, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, CityId, Name, Notification, LastCheckedAt, ShouldContain, ShouldNotContain, PricePerMeterFrom, PricePerMeterTo, DistrictId, AllowFromAgency)
                        VALUES(@UserId, @Property, @Deal, @Market, @FlatAreaFrom, @FlatAreaTo, @PriceFrom, @PriceTo, @CityId, @Name, @Notification, @LastCheckedAt, @ShouldContain, @ShouldNotContain, @PricePerMeterFrom, @PricePerMeterTo, @DistrictId, @AllowFromAgency);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        filter.UserId,
                        filter.Property,
                        filter.Deal,
                        filter.Market,
                        FlatAreaFrom = filter.FlatAreaFrom.ToNull(),
                        FlatAreaTo = filter.FlatAreaTo.ToNull(),
                        PriceFrom = filter.PriceFrom.ToNull(),
                        PriceTo = filter.PriceTo.ToNull(),
                        filter.CityId,
                        filter.Name,
                        filter.Notification,
                        filter.LastCheckedAt,
                        ShouldContain = string.IsNullOrWhiteSpace(filter.ShouldContain) ? default(string) : filter.ShouldContain.ToLowerInvariant(),
                        ShouldNotContain = string.IsNullOrWhiteSpace(filter.ShouldNotContain) ? default(string) : filter.ShouldNotContain.ToLowerInvariant(),
                        PricePerMeterFrom = filter.PricePerMeterFrom.ToNull(),
                        PricePerMeterTo = filter.PricePerMeterTo.ToNull(),
                        DistrictId = filter.DistrictId.ToNull(),
                        filter.AllowFromAgency
                    });
            }
        }

        public async Task<int> DeleteAsync(int filterId, int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                using (var trans = c.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    await c.ExecuteAsync(
                        @"DELETE FROM OfferFilters WHERE FilterId = @Id",
                        new
                        {
                            Id = filterId
                        }, trans);

                    await c.ExecuteAsync(
                        @"DELETE FROM Filters WHERE Id = @Id AND UserId = @UserId",
                        new
                        {
                            Id = filterId,
                            UserId = userId
                        }, trans);

                    trans.Commit();

                    return 0;
                }
            }
        }

        public async Task<int> UpdateAsync(Filter filter)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Filters SET Property = @Property, Deal = @Deal, Market = @Market, CityId = @CityId, FlatAreaFrom = @FlatAreaFrom, FlatAreaTo = @FlatAreaTo, PriceFrom = @PriceFrom, PriceTo = @PriceTo, Name = @Name, Notification = @Notification, ShouldContain = @ShouldContain, ShouldNotContain = @ShouldNotContain, PricePerMeterFrom = @PricePerMeterFrom, PricePerMeterTo = @PricePerMeterTo, DistrictId = @DistrictId, AllowFromAgency = @AllowFromAgency 
                        WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        filter.Id,
                        filter.UserId,
                        filter.Property,
                        filter.Deal,
                        filter.Market,
                        filter.CityId,
                        FlatAreaFrom = filter.FlatAreaFrom.ToNull(),
                        FlatAreaTo = filter.FlatAreaTo.ToNull(),
                        PriceFrom = filter.PriceFrom.ToNull(),
                        PriceTo = filter.PriceTo.ToNull(),
                        filter.Name,
                        filter.Notification,
                        ShouldContain = string.IsNullOrWhiteSpace(filter.ShouldContain) ? default(string) : filter.ShouldContain.ToLowerInvariant(),
                        ShouldNotContain = string.IsNullOrWhiteSpace(filter.ShouldNotContain) ? default(string) : filter.ShouldNotContain.ToLowerInvariant(),
                        PricePerMeterFrom = filter.PricePerMeterFrom.ToNull(),
                        PricePerMeterTo = filter.PricePerMeterTo.ToNull(),
                        DistrictId = filter.DistrictId.ToNull(),
                        filter.AllowFromAgency
                    });
            }
        }

        public async Task<Filter> GetAsync(int filterId, int userId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryFirstOrDefaultAsync<Filter>(
                    @"SELECT Id, UserId, Property, Deal, Market, CityId, FlatAreaFrom, FlatAreaTo, PriceFrom, PriceTo, Name, Notification, LastCheckedAt, ShouldContain, ShouldNotContain, PricePerMeterFrom, PricePerMeterTo, DistrictId, AllowPrivate, AllowFromAgency FROM Filters 
                        WHERE Id = @Id AND UserId = @UserId",
                    new
                    {
                        Id = filterId,
                        UserId = userId
                    });
            }
        }

        public async Task<int> DeleteAsync(int filterId)
        {
            using (var c = _dbAccess.Connection)
            {
                using (var trans = c.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    await c.ExecuteAsync(
                        @"DELETE FROM OfferFilters WHERE FilterId = @Id",
                        new
                        {
                            Id = filterId
                        }, trans);

                    await c.ExecuteAsync(
                        @"DELETE FROM Filters WHERE Id = @Id",
                        new
                        {
                            Id = filterId
                        }, trans);

                    trans.Commit();

                    return 0;
                }
            }
        }
    }
}