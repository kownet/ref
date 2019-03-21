using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Poc
{
    public class DeleteFilter
    {
        public class Cmd : IRequest<Result>
        {
            public Cmd(int filterId)
            {
                FilterId = filterId;
            }

            public int FilterId { get; private set; }
        }

        public class Result : BaseResult { }

        public class Handler : IRequestHandler<Cmd, Result>
        {
            private readonly IFilterRepository _filterRepository;

            public Handler(IFilterRepository filterRepository)
            {
                _filterRepository = filterRepository;
            }

            public async Task<Result> Handle(Cmd request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = (await _filterRepository.FindByAsync(f => f.Id == request.FilterId)).FirstOrDefault();

                    if (entity is null)
                    {
                        return new Result { Message = "No such filter" };
                    }

                    await _filterRepository.DeleteAsync(request.FilterId);

                    return new Result();
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}