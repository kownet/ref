using MediatR;
using Ref.Data.Repositories;
using Ref.Services.Features.Shared;
using Ref.Shared.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Email
    {
        public class Cmd : IRequest<Result>
        {
            public int Id { get; set; }
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
                    return new Result { Message = "Proszę podać poprawny adres email." };

                try
                {
                    var entity = await _userRepository.GetAsync(request.Id);

                    if (entity is null)
                    {
                        return new Result { Message = "Nie ma takiego usera" };
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