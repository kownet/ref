using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.AdminInfos
{
    public class All
    {
        public class Query : IRequest<Result> { }

        public class Result : BaseResult
        {
            public Result()
            {
                Infos = new HashSet<AdminInfoResult>();
            }

            public IEnumerable<AdminInfoResult> Infos { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IAdminInfosRepository _adminInfosRepository;

            public Handler(IAdminInfosRepository adminInfosRepository)
            {
                _adminInfosRepository = adminInfosRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var entities = await _adminInfosRepository.FindByAsync(i => i.IsActive);
                    var result = new List<AdminInfoResult>();

                    if (entities.AnyAndNotNull())
                    {
                        foreach (var entity in entities)
                        {
                            result.Add(new AdminInfoResult { Id = entity.Id, Text = entity.Text, IsActive = entity.IsActive, DateAdded = entity.DateAdded.Format() });
                        }

                        return new Result { Infos = result };
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