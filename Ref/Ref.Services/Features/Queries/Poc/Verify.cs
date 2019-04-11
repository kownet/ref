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
            public bool IsActive { get; set; }
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
                    return new Result { Message = "Proszę o podanie swojego identyfikatora celem weryfikacji" };

                try
                {
                    var user = (await _userRepository.FindByAsync(u => u.Guid == request.Guid.ToUpperInvariant())).FirstOrDefault();

                    if (user is null)
                        return new Result { Message = "Nie ma takiego użytkownika" };
                    else
                    {
                        if (user.DemoPassed)
                            return new Result { Message = "Twój okres próbny minął. Prosimy o kontakt w celu dalszego korzystania z usługi." };

                        return new Result { UserId = user.Id, Email = user.Email, RegisteredAt = user.RegisteredAt.Format(), IsActive = user.IsActive };
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