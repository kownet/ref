using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Events
{
    public class All
    {
        public class Query : IRequest<Result> { }

        public class Result : BaseResult
        {
            public Result()
            {
                Events = new HashSet<EventResult>();
            }

            public IEnumerable<EventResult> Events { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IEventRepository _eventRepository;
            private readonly ISiteRepository _siteRepository;

            public Handler(
                IEventRepository eventRepository,
                ISiteRepository siteRepository)
            {
                _eventRepository = eventRepository;
                _siteRepository = siteRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var activeSites = await _siteRepository.FindByAsync(s => s.IsActive);

                    var entities = await _eventRepository.GetAllAsync();
                    var result = new List<EventResult>();

                    var activeEvents = new List<Event>();

                    activeEvents.AddRange(entities.Where(e => activeSites.Any(a => (int)a.Type == (int)e.Category)));
                    activeEvents.AddRange(entities.Where(e => activeSites.Any(a => (int)a.Type == ((int)e.Category - 100))));

                    if (activeEvents.AnyAndNotNull())
                    {
                        foreach (var entity in activeEvents)
                        {
                            result.Add(new EventResult
                            {
                                UpdatedAt = entity.UpdatedAt.Format(),
                                Category = entity.Category.ToString(),
                                Type = entity.Type.ToString()
                            });
                        }

                        return new Result { Events = result.OrderByDescending(i => i.UpdatedAt) };
                    }
                    else return new Result { Message = "No events in database" };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}