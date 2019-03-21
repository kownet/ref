using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Poc
{
    public class Verify
    {
        public class Query : IRequest<Result>
        {
            public string Guid { get; set; }
        }

        public class Result : BaseResult
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string RegisteredAt { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Guid))
                    return new Result { Message = "Please provide GUID to verify your identity." };

                try
                {
                    var user = (await _userRepository.FindByAsync(u => u.Guid == request.Guid.ToUpperInvariant())).FirstOrDefault();

                    if (user is null)
                        return new Result { Message = "No such user." };
                    else
                    {
                        return new Result { UserId = user.Id, Email = user.Email, RegisteredAt = user.RegisteredAt.Format() };
                    }
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}