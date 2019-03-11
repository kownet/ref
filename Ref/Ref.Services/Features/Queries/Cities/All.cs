using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Cities
{
    public class All
    {
        public class Query : IRequest<Result> { }

        public class Result : BaseResult
        {
            public Result()
            {
                Cities = new HashSet<CityResult>();
            }

            public IEnumerable<CityResult> Cities { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly ICitiesRepository _citiesRepository;

            public Handler(ICitiesRepository citiesRepository)
            {
                _citiesRepository = citiesRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var entities = await _citiesRepository.GetAllAsync();
                    var result = new List<CityResult>();

                    if (entities.AnyAndNotNull())
                    {
                        foreach (var entity in entities)
                        {
                            result.Add(new CityResult { Id = entity.Id, Name = entity.Name, NameRaw = entity.NameRaw });
                        }

                        return new Result { Cities = result };
                    }
                    else return new Result { Message = "No cities in database" };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}