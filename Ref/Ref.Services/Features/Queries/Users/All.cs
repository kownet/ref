using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Queries.Users
{
    public class All
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public Result()
            {
                Users = new HashSet<UserResult>();
            }

            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public IEnumerable<UserResult> Users { get; set; }
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
                    var entities = await _userRepository.GetAllAsync();
                    var result = new List<UserResult>();

                    if (entities.AnyAndNotNull())
                    {
                        foreach (var entity in entities)
                        {
                            result.Add(new UserResult { Id = entity.Id, Email = entity.Email });
                        }

                        return new Result
                        {
                            Users = result
                        };
                    }
                    else return new Result { Message = "No users in database" };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}