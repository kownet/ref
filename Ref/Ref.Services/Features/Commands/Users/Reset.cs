using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Reset
    {
        public class Cmd : IRequest<Result>
        {
            public int Id { get; set; }
            public string Password { get; set; }
            public string Confirmation { get; set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }
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
                if (!string.Equals(request.Password, request.Confirmation))
                    return new Result { Message = "Password and password confirmation are not equal" };

                try
                {
                    var entity = await _userRepository.GetAsync(request.Id);

                    if (entity is null)
                    {
                        return new Result { Message = "No such user" };
                    }

                    _passwordProvider.CreatePasswordHash(
                            request.Password,
                            out byte[] passwordHash,
                            out byte[] passwordSalt);

                    entity.PasswordHash = passwordHash;
                    entity.PasswordSalt = passwordSalt;

                    await _userRepository.UpdateAsync(entity);

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