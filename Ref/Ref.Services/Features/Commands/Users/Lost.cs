﻿using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Lost
    {
        public class Cmd : IRequest<Result>
        {
            public string Email { get; set; }
        }

        public class Result : BaseResult { }

        public class Handler : IRequestHandler<Cmd, Result>
        {
            private readonly IUserRepository _userRepository;

            public Handler(
                IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<Result> Handle(Cmd request, CancellationToken cancellationToken)
            {
                if (!request.Email.IsValidEmail())
                    return new Result { Message = "Please provide valid email" };

                try
                {
                    var entity = (await _userRepository.FindByAsync(u => u.Email == request.Email.ToLowerInvariant())).FirstOrDefault();

                    if (entity is null)
                    {
                        return new Result
                        {
                            Message = "User not exist"
                        };
                    }

                    //sent email

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