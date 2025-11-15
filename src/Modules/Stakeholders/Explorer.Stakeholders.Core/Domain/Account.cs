using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum AccountRole
    {
        Admin,
        Author,
        Tourist
    }

    public enum AccountStatus
    {
        Active,
        Blocked
    }
    public class Account
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public AccountRole Role { get; private set; }
        public AccountStatus Status { get; private set; }

        protected Account() { } 

        public Account(string username, string password, string email, AccountRole role)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            Status = AccountStatus.Active;
        }

        public void Block()
        {
            Status = AccountStatus.Blocked;
        }

        public void Unblock()
        {
            Status = AccountStatus.Active;
        }
    }
}
