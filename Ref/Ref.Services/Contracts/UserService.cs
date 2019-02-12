using Ref.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ref.Services.Contracts
{
    public interface IUserService
    {
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        public User GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}