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

            var user = _context.Users.SingleOrDefault(x => x.UserName == username)

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }
    }
}