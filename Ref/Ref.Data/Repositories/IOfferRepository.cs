using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IOfferRepository : IRepository
    {
        Task<IQueryable<Offer>> FindByAsync(Expression<Func<Offer, bool>> predicate);
    }

    public class OfferRepository : IOfferRepository
    {
        private readonly IDbAccess _dbAccess;

        public OfferRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
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