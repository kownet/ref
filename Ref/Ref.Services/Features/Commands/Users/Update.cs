using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Update
    {
        public class Cmd : IRequest<Result>
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Result : BaseResult { }

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
                if (!request.Email.IsValidEmail())
                    return new Result { Message = "Please provide valid email" };

                if (string.IsNullOrWhiteSpace(request.Password))
                    return new Result { Message = "Please provide current password" };

                try
                {
                    var entity = await _userRepository.GetAsync(request.Id);

                    if (entity is null)
                    {
                        return new Result { Message = "No such user" };
                    }

                    var user = _passwordProvider.VerifyPasswordHash(request.Password, entity.PasswordHash, entity.PasswordSalt);

                    if (!user)
                    {
                        return new Result { Message = "Bad username or password" };
                    }

                    entity.Email = request.Email;

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