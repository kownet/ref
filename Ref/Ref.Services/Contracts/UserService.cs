using Ref.Data.Models;
using Ref.Data.Repositories;
using Ref.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ref.Services.Contracts
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetById(int id)
            => _userRepository.Get(id);

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _userRepository.GetAll().SingleOrDefault(x => x.Email == email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
            => _userRepository.GetAll();

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_userRepository.GetAll().Any(x => x.Email == user.Email))
                throw new AppException("Username \"" + user.Email + "\" is already taken");

            byte[] passwordSalt;
            CreatePasswordHash(password, out byte[] passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepository.Create(user);

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Email != user.Email)
            {
                // username has changed so check if the new username is already taken
                if (_userRepository.GetAll().Any(x => x.Email == userParam.Email))
                    throw new AppException("Username " + userParam.Email + " is already taken");
            }

            // update user properties


            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _userRepository.Update(user);
        }

        public void Delete(int id)
        {
            var user = _userRepository.GetAll().FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _userRepository.Delete(user.Id);
            }
        }

        #region Private
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        #endregion
    }
}