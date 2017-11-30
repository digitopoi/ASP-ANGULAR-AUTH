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
        User IUserService(DataContext context)
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

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User UserGetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {

            byte[] passwordHash, passwordSet;

            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException("Password is required.");

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new AppException("Username " + user.UserName + " is already taken");

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
            
        }
    }
}