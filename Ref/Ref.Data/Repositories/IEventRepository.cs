﻿using Dapper;
using Ref.Data.Db;
using Ref.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ref.Data.Repositories
{
    public interface IEventRepository : IRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<IQueryable<Event>> FindByAsync(Expression<Func<Event, bool>> predicate);
        Task<int> CreateAsync(Event ev);
        Task<int> DeleteAsync(int evId);
        Task<int> UpdateAsync(Event ev);
        Task<Event> GetAsync(int evId);
        Task<int> Upsert(Event ev);
    }

    public class EventRepository : IEventRepository
    {
        private readonly IDbAccess _dbAccess;

        public EventRepository(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public async Task<int> CreateAsync(Event ev)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"INSERT INTO Events (Category, Type, UpdatedAt, Message)
                        VALUES(@Category, @Type, @UpdatedAt, @Message);
                    SELECT CAST(SCOPE_IDENTITY() as int)",
                    new
                    {
                        ev.Category,
                        ev.Type,
                        UpdatedAt = DateTime.Now,
                        ev.Message
                    });
            }
        }

        public async Task<int> DeleteAsync(int evId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"DELETE FROM Events WHERE Id = @EventId",
                    new
                    {
                        EventId = evId
                    });
            }
        }

        public async Task<IQueryable<Event>> FindByAsync(Expression<Func<Event, bool>> predicate)
        {
            using (var c = _dbAccess.Connection)
            {
                var result = (await c.QueryAsync<Event>(
                    @"SELECT Id, Category, Type, UpdatedAt, Message FROM Events")).AsQueryable();

                return result.Where(predicate);
            }
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryAsync<Event>(
                    @"SELECT Id, Category, Type, UpdatedAt, Message FROM Events");
            }
        }

        public async Task<Event> GetAsync(int evId)
        {
            using (var c = _dbAccess.Connection)
            {
                return await c.QueryFirstOrDefaultAsync<Event>(
                    @"SELECT Id, Category, Type, UpdatedAt, Message FROM Events 
                        WHERE Id = @Id",
                    new
                    {
                        Id = evId
                    });
            }
        }

        public async Task<int> Upsert(Event ev)
        {
            using (var c = _dbAccess.Connection)
            {
                var isAlready = await c.QueryFirstOrDefaultAsync<Event>(
                    @"SELECT Id, Category, Type, UpdatedAt, Message FROM Events 
                        WHERE Category = @Category AND Type = @Type",
                    new
                    {
                        ev.Category,
                        ev.Type
                    });

                if (!(isAlready is null))
                {
                     await c.ExecuteAsync(
                            @"UPDATE Events SET UpdatedAt = @UpdatedAt, Message = @Message WHERE Id = @Id",
                            new
                            {
                                isAlready.Id,
                                UpdatedAt = DateTime.Now,
                                ev.Message
                            });
                    
                }
                else
                {
                    await c.ExecuteAsync(
                    @"INSERT INTO Events (Category, Type, UpdatedAt, Message)
                        VALUES(@Category, @Type, @UpdatedAt, @Message)",
                    new
                    {
                        ev.Category,
                        ev.Type,
                        UpdatedAt = DateTime.Now,
                        ev.Message
                    });
                }
            }
            return 0;
        }

        public async Task<int> UpdateAsync(Event ev)
        {
            using(var c = _dbAccess.Connection)
            {
                return await c.ExecuteAsync(
                    @"UPDATE Events SET Category = @Category, Type = @Type, UpdatedAt = @UpdatedAt, Message = @Message 
                        WHERE Id = @Id",
                    new
                    {
                        ev.Id,
                        ev.Category,
                        ev.Type,
                        ev.UpdatedAt,
                        ev.Message
                    });
            }
        }
    }
}