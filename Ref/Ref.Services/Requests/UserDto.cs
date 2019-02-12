using Ref.Data.Models;

namespace Ref.Services.Requests
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User ToEntity()
            => new User
            {
                Id = Id,
                Email = Email
            };
    }
}