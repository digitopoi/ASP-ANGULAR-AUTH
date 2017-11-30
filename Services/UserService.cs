using System;
using System.Collections.Generic;
using System.Linq;
using ASP_Angular_Auth.Data;
using ASP_Angular_Auth.Models;

namespace ASP_Angular_Auth.Services
{
    public class UserService : IUserService
    {
        private  DataContext _context;
        private HashService _hashService;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;

            var user = _context.Users.SingleOrDefault(x => x.UserName == username);

            if (user == null) {
                return null;
            }

            if (!_hashService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {

            byte[] passwordHash, passwordSalt;

            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException("Password is required.");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username " + user.UserName + " is already taken");

            _hashService.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                //  username has changed - check if the new username is already in use
                if (_context.Users.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken.")
            }

            //  update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;

            //  update password if entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                _hashService.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

    }
}