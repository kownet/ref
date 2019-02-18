using MediatR;
using Microsoft.IdentityModel.Tokens;
using Ref.Data.Repositories;
using Ref.Services.Contracts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ref.Services.Features.Commands.Users
{
    public class Authenticate
    {
        public class Cmd : IRequest<Result>
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string SigningToken { get; set; }
        }

        public class Result
        {
            public bool Succeed => string.IsNullOrWhiteSpace(Message);
            public string Message { get; set; }

            public int Id { get; set; }
            public string Email { get; set; }
            public string Token { get; set; }
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
                var entity = (await _userRepository.FindByAsync(u => u.Email == request.Email)).FirstOrDefault();

                if (entity is null)
                {
                    return new Result
                    {
                        Message = "User not exist"
                    };
                }

                var user = _passwordProvider.VerifyPasswordHash(request.Password, entity.PasswordHash, entity.PasswordSalt);

                if (!user)
                {
                    return new Result
                    {
                        Message = "Bad username or password"
                    };
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(request.SigningToken);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, entity.Id.ToString()),
                        new Claim(ClaimTypes.Role, entity.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new Result
                {
                    Id = entity.Id,
                    Email = entity.Email,
                    Token = tokenString
                };
            }
        }
    }
}