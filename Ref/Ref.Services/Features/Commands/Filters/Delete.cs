using MediatR;
using Ref.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Filters
{
    public class Delete
    {
        public class Cmd : IRequest<Result>
        {
            public Cmd(int filterId, int userId)
            {
                FilterId = filterId;
                UserId = userId;
            }

            public int FilterId { get; private set; }
            public int UserId { get; private set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }
        }

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
                    var entity = await _filterRepository.GetAsync(request.FilterId, request.UserId);

                    if (entity is null)
                    {
                        return new Result { Message = "No such filter" };
                    }

                    await _filterRepository.DeleteAsync(request.FilterId, request.UserId);

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