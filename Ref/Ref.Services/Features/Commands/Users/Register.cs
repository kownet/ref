using MediatR;
using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
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
            public string Confirmation { get; set; }
        }

        public class Result : BaseResult
        {
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
                if(!string.Equals(request.Password, request.Confirmation))
                    return new Result { Message = "Password and password confirmation are not equal" };

                if (!request.Email.IsValidEmail())
                    return new Result { Message = "Please provide valid email" };

                try
                {
                    _passwordProvider.CreatePasswordHash(
                        request.Password,
                        out byte[] passwordHash,
                        out byte[] passwordSalt);

                    var result = await _userRepository.CreateAsync(new User
                    {
                        Email = request.Email.ToLowerInvariant(),
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        Role = Role.User,
                        RegisteredAt = DateTime.Now,
                        Subscription = SubscriptionType.Normal,
                        IsActive = true,
                        Guid = Guid.NewGuid().ToString().ToUpperInvariant()
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