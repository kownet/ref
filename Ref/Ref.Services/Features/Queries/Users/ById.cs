using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Users
{
    public class ById
    {
        public class Query : IRequest<Result>
        {
            public Query(int userId)
            {
                UserId = userId;
            }

            public int UserId { get; private set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public UserResult User { get; set; }
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
                try
                {
                    var user = await _userRepository.GetAsync(request.UserId);

                    if (user is null)
                    {
                        return new Result { Message = "No such user" };
                    }
                    else
                    {
                        return new Result
                        {
                            User = new UserResult
                            {
                                Id = user.Id,
                                Email = user.Email
                            }
                        };
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