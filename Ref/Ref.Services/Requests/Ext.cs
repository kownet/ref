using Ref.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ref.Services.Requests
{
    public static class EntitesExt
    {
        public static UserDto ToDto(this User user)
            => new UserDto
            {
                Id = user.Id,
                Email = user.Email
            };

        public static IEnumerable<UserDto> ToDto(this IEnumerable<User> agents)
            => agents.Select(vm => vm.ToDto());
    }
}