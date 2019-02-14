using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Register
    {
        public class Cmd : IRequest<Result>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public int UserId { get; set; }
        }

        public class Handler : IRequestHandler<Cmd, Result>
        {
            private readonly IPasswordProvider _passwordProvider;
            private readonly IUserRepository _userRepository;

            public Handler(
                IPasswordProvider passwordProvider,
                IUserRepository userRepository)
            {
                _passwordProvider = passwordProvider;
                _userRepository = userRepository;
            }

            public async Task<Result> Handle(Cmd request, CancellationToken cancellationToken)
            {
                try
                {
                    _passwordProvider.CreatePasswordHash(
                        request.Password,
                        out byte[] passwordHash,
                        out byte[] passwordSalt);

                    var result = await _userRepository.Create(new User
                    {
                        Email = request.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    });

                    return new Result
                    {
                        UserId = result
                    };
                }
                catch (Exception ex)
                {
                    return new Result { Message = ex.Message };
                }
            }
        }
    }
}