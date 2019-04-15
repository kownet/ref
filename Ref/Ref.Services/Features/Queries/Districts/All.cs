using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Districts
{
    public class All
    {
        public class Query : IRequest<Result> { }

        public class Result : BaseResult
        {
            public Result()
            {
                Districts = new HashSet<DistrictResult>();
            }

            public IEnumerable<DistrictResult> Districts { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IDistrictRepository _districtRepository;

            public Handler(IDistrictRepository districtRepository)
            {
                _districtRepository = districtRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var entities = await _districtRepository.GetAllAsync();
                    var result = new List<DistrictResult>();

                    if (entities.AnyAndNotNull())
                    {
                        foreach (var entity in entities)
                        {
                            result.Add(new DistrictResult { Id = entity.Id, Name = entity.Name, NameRaw = entity.NameRaw });
                        }

                        return new Result { Districts = result };
                    }
                    else return new Result { Message = "No districts in database" };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}