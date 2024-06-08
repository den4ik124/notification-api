using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApplication.Core.newFolder
{
    public class User
    {
        private User(Guid id, string userName, string passwordHash, string email)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            Email = email;
        }

        public Guid Id { get; }

        public string UserName { get; }

        public string PasswordHash { get; }

        public string Email { get; }

        public static User Create(Guid id, string userName, string passwordHash, string email)
        {
            return new User(id, userName, passwordHash, email);
        }
    }
}